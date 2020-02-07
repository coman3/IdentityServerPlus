using IdentityServerPlus.Plugin.Base.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Matching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IdentityServerPlus.Plugin.Base.Services
{
    class PluginEndpointSelector : EndpointSelector
    {
        private PluginManager _pluginManager { get; }

        public PluginEndpointSelector(PluginManager pluginManager)
        {
            _pluginManager = pluginManager;
        }

        public override Task SelectAsync(
                    HttpContext httpContext,
                    CandidateSet candidateSet)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            if (candidateSet == null)
            {
                throw new ArgumentNullException(nameof(candidateSet));
            }

            Select(httpContext, candidateSet);
            return Task.CompletedTask;
        }

        internal void Select(HttpContext httpContext, CandidateSet candidateSet)
        {
            // Fast path: We can specialize for trivial numbers of candidates since there can
            // be no ambiguities
            switch (candidateSet.Count)
            {
                case 0:
                    {
                        // Do nothing
                        break;
                    }

                case 1:
                    {
                        ref var state = ref candidateSet[0];
                        if (candidateSet.IsValidCandidate(0))
                        {
                            httpContext.SetEndpoint(state.Endpoint);
                            httpContext.Request.RouteValues = state.Values;
                        }

                        break;
                    }

                default:
                    {
                        // Slow path: There's more than one candidate (to say nothing of validity) so we
                        // have to process for ambiguities.
                        ProcessFinalCandidates(httpContext, candidateSet);
                        break;
                    }
            }
        }

        private int GetIndexForMatch(CandidateState state)
        {
            //TODO: Cache this, as this will always be the same once loaded
            var position = state.Endpoint.DisplayName.IndexOf('(');
            var type = state.Endpoint.DisplayName.Substring(position + 1, state.Endpoint.DisplayName.IndexOf(')') - position - 1);
            var ass = _pluginManager.PluginInstances.FirstOrDefault(x => x.Assembly.FullName.Contains(type));
            if (ass == null) return 0;

            var themeIndex = ass.Providers.OfType<IThemeProvider>().Max(x => x.Index);
            return themeIndex;
        }

        private void ProcessFinalCandidates(
            HttpContext httpContext,
            CandidateSet candidateSet)
        {
            Endpoint endpoint = null;
            RouteValueDictionary values = null;
            int? foundScore = null;
            for (var i = 0; i < candidateSet.Count; i++)
            {
                ref var state = ref candidateSet[i];
                if (!candidateSet.IsValidCandidate(i))
                {
                    continue;
                }
                var indexScore = GetIndexForMatch(state);


                if (foundScore == null)
                {
                    // This is the first match we've seen - speculatively assign it.
                    endpoint = state.Endpoint;
                    values = state.Values;
                    foundScore = state.Score + GetIndexForMatch(state);
                }
                else if (foundScore < state.Score + indexScore)
                {
                    // This candidate is lower priority than the one we've seen
                    // so far, we can stop.
                    //
                    // Don't worry about the 'null < state.Score' case, it returns false.
                    endpoint = state.Endpoint;
                    values = state.Values;
                    break;
                }
                else if (foundScore == state.Score + indexScore)
                {
                    // This is the second match we've found of the same score, so there
                    // must be an ambiguity.
                    //
                    // Don't worry about the 'null == state.Score' case, it returns false.

                    ReportAmbiguity(candidateSet);

                    // Unreachable, ReportAmbiguity always throws.
                    throw new NotSupportedException();
                }
            }

            if (endpoint != null)
            {
                httpContext.SetEndpoint(endpoint);
                httpContext.Request.RouteValues = values;
            }
        }

        private static void ReportAmbiguity(CandidateSet candidateSet)
        {
            // If we get here it's the result of an ambiguity - we're OK with this
            // being a littler slower and more allocatey.
            var matches = new List<Endpoint>();
            for (var i = 0; i < candidateSet.Count; i++)
            {
                ref var state = ref candidateSet[i];
                if (candidateSet.IsValidCandidate(i))
                {
                    matches.Add(state.Endpoint);
                }
            }

            var message = Environment.NewLine + string.Join(
                "," + Environment.NewLine,
                string.Join(Environment.NewLine, matches.Select(e => e.DisplayName)));
            throw new AmbiguousMatchException(message);
        }
    }
}

document.addEventListener("DOMContentLoaded", onLoad);

var loginTab, signupTab, clickLoaders = null;

function onLoad() {
    window.addEventListener("popstate", function (event) {
        event.preventDefault();
    });

    setTimeout(function () {
        document.getElementsByTagName("body")[0].className = "";
    }, 10);


    loginTab = document.getElementById("login-tab");
    signupTab = document.getElementById("signup-tab");

    loginTab.addEventListener("click", function () {
        if (!hasClass(loginTab, "active")) {
            loginTab.className += " active";
            signupTab.className = signupTab.className.replace(" active", "");
            window.history.pushState({ id: "register" }, "", "/Account/Login");
        }
    });
    signupTab.addEventListener("click", function () {
        if (!hasClass(signupTab, "active")) {
            signupTab.className += " active";
            loginTab.className = loginTab.className.replace(" active", "");
            window.history.pushState({ id:"register" }, "", "/Account/Register");

        }
    });

    clickLoaders = document.getElementsByClassName("click-loader");
    for (let index = 0; index < clickLoaders.length; index++) {
        let element = clickLoaders[index];
        element.addEventListener("click", function (event) {
            let oldHtml = element.innerHTML;
            element.className += " round";
            element.innerHTML = "<div class='sbl-circ-path'></div>";
            if (element.beenClicked === true)
                event.preventDefault();
            else {
                element.beenClicked = true;
                setTimeout(function () {
                    element.className = element.className.replace(" round", "");
                    element.innerHTML = oldHtml;
                    element.beenClicked = false;
                }, 3000);
            }
        });
    }
}

function hasClass(element, cls) {
    return (' ' + element.className + ' ').indexOf(' ' + cls + ' ') > -1;
}
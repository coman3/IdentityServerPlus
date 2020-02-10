document.addEventListener("DOMContentLoaded", onLoad);

var loginTab, signupTab, clickLoaders = null;

function onLoad() {

    setTimeout(function () {
        document.getElementsByTagName("body")[0].className = "";
    }, 10);


    loginTab = document.getElementById("login-tab");
    signupTab = document.getElementById("signup-tab");

    loginTab.addEventListener("click", function () {
        if (!hasClass(loginTab, "active")) {
            loginTab.className += " active";
            signupTab.className = signupTab.className.replace(" active", "");
        }
    });
    signupTab.addEventListener("click", function () {
        if (!hasClass(signupTab, "active")) {
            signupTab.className += " active";
            loginTab.className = loginTab.className.replace(" active", "");
        }
    });

    clickLoaders = document.getElementsByClassName("click-loader");
    for (let index = 0; index < clickLoaders.length; index++) {
        let element = clickLoaders[index];
        element.addEventListener("click", function (event) {
            let oldHtml = element.innerHTML;
            element.className += " round";
            element.innerHTML = "<div class='sbl-circ-path'></div>";
            if (element.disabled === true)
                event.preventDefault();
            else {
                element.disabled = true;
                setTimeout(function () {
                    element.className = element.className.replace(" round", "");
                    element.innerHTML = oldHtml;
                    element.disabled = false;
                }, 3000);
            }
        });
    }
}

function hasClass(element, cls) {
    return (' ' + element.className + ' ').indexOf(' ' + cls + ' ') > -1;
}
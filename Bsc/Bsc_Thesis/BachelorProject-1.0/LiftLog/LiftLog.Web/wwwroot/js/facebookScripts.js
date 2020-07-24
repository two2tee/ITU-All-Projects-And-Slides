window.fbAsyncInit = function() {
    FB.init({
        appId: "1807178839603521",
        version: "v2.8",
        status: true,
        xfbml: true
    });

    FB.getLoginStatus(function(response) {
        Console.log("init!");
    });
};


(function(doc) {
    var js;
    var id = "facebook-jssdk";
    var ref = doc.getElementsByTagName("script")[0];
    if (doc.getElementById(id)) {
        return;
    }
    js = doc.createElement("script");
    js.id = id;
    js.async = true;
    js.src = "//connect.facebook.net/en_US/all.js";
    ref.parentNode.insertBefore(js, ref);
}(document));

function Share(message) {
    FB.login(
        function(response) {
            if (response.authResponse) {
                FB.api("/me",
                    function(response) {
                        console.log(response.name);
                    });

                FB.api("/me/feed",
                    "post",
                    {
                        message: message
                    });
                $("#shareModal").modal(); //Todo Remmber to add shareModal on your page


            } else {
                alert("Login attempt failed!");
            }
        },
        { scope: "publish_actions" });
}

function Logout() {
    FB.logout(function() { document.location.reload(); });
}
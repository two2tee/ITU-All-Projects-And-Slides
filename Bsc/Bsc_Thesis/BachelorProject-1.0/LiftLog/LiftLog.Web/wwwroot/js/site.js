
/* Site js */

/* 
Self-executing nameless/anonymous function executed on startup
(); is used to execute the function
No name is given to avoid name colission with other JS code in global scope 
Code executes outside global scope 
*/
(function() {

    // Retrieve wrapper and side bar panel, returned as wrapped set of DOM elements 
    // Use '$'to denote it is a jQuery object 
    var $sidebarAndWrapper = $("#sidebar, #wrapper");
    var $icon = $("#sidebarToggle i.fa"); // Go find sidebar toggle and as children find italic classed with fa 

    // Do work directly on toggle button because it is not used again (otherwise have var) 
    $("#sidebarToggle").on("click",
        function() {
            // Add or remove class tag to wrapper and sidebar
            // When sidebar ir present change web page look
            $sidebarAndWrapper.toggleClass("hide-sidebar"); // Class that we wil toggle (add or remove) 

            // Display text to be Hide when Side bar exists, vice versa 
            if ($sidebarAndWrapper.hasClass("hide-sidebar")) {
                //$(this).text("Show Sidebar");
                $icon.removeClass("fa-angle-left");
                $icon.addClass("fa-angle-right");
            } else {
                //$(this).text("Hide Sidebar");
                $icon.removeClass("fa-angle-right");
                $icon.addClass("fa-angle-left");
            }
        });

})();
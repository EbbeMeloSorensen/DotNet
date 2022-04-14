// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function dummyFunction() {
    // Dollartegnet indikerer, at vi bruger en funktion fra jquery
    $.ajax({
        //url: "http://api.exchangeratesapi.io/v1/latest?access_key=3d5861a43985fbc3778eda7aaa2fefa9&symbols=USD,INR",
        url: "https://localhost:44358/api/artists",
        type: "GET",
        dataType: "json",
        success: function(result) {
            //heroes = result. // "We set our gloal heroes list
            //drawHeroTable(result);
            //document.getElementById("output").innerHTML = (3.1415927).toFixed(2);
        }
    });

    document.getElementById("output").innerHTML = (3.1415927).toFixed(2);
}

// we will use jquery and ajax to make an asynchronous call to the web service. Jquery is a powerful javascript library 
// which makes many javascript functionality easier, especially when it comes to browser compatibility, You can also
// access DOM elements with jQuery faster than with plain javascript.
// Ajax is just a set of web development techniques that allows us to do the upcoming calls to our web service. Ajax
// stands for "Asyncrhonous Javascript and XML"

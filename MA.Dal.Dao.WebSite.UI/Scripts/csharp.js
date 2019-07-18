setTimeout(function () {
    if (disableCsharp == false) {
        var allCSharpDOMs = document.querySelectorAll('.csharp-code-example span:not(:empty):not([style="mso-spacerun:yes"]):not(:first-child),.csharp-code-example span:not(:empty)[style*="font-family:Consolas;mso-bidi-font-family:Consolas;color:blue"]');
        for (var i = 0; i < allCSharpDOMs.length; i++) {
            var item = allCSharpDOMs[i];
            item.innerHTML = item.innerHTML + "&nbsp;";
        }
    }
});
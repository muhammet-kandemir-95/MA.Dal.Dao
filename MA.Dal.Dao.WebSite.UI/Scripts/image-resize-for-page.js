var normalTextImgs = document.querySelectorAll(".normal-text-img img");
for (var i = 0; i < normalTextImgs.length; i++) {
    var item = normalTextImgs[i];
    item.style.maxHeight = ((window.innerHeight / 100) * 70) + "px";
}
// JavaScript injection
//document.body.click();
const intervalId = setInterval(() => {
    const button = document.querySelector('.ytp-large-play-button');
    if (button == null) return;
    const video = document.querySelector('.video-stream.html5-main-video');
    if (video == null) return;
    if (button.parentElement.style.display != "none" && video.paused) button.click();
    clearInterval(intervalId);
}, 500);
// 自動再生無効
const intervalId2 = setInterval(() => {
    const button = document.querySelector('.ytp-autonav-toggle-button');
    if (button == null) return;
    const intervalId3 = setInterval(() => {
        button.parentNode.parentNode.click();
        if (button.ariaChecked == 'false') clearInterval(intervalId3);
    }, 500);
    clearInterval(intervalId2);
}, 500);

//document.querySelectorAll("yt-confirm-dialog-renderer button")[0].

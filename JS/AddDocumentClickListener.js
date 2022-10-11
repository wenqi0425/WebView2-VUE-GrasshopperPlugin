document.addEventListener('click', function (event) {
    const element = event.target;
    const returnObject =
    {
        type: 'click',
        targetId: element.id,
        targetName: element.name,
        targetType: element.type,
        targetValue: element.value,
        targetTagName: element.tagName
    }

    window.chrome.webview.postMessage(returnObject);
});

document.addEventListener('keydown', function (event) {
    const returnObject =
    {
        type: 'keydown'
    };
    window.chrome.webview.postMessage(returnObject);
});

document.addEventListener('keypress', function (event) {
    const returnObject =
    {
        type: 'keypress'
    };
    window.chrome.webview.postMessage(returnObject);
});
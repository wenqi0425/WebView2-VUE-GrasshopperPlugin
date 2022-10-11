function setValues(elementId, value) {
    const element = document.getElementById(elementId);

    if (element === null) return;

    // if it's text
    if (element.innerText) {
        element.innerText = value;
    }

    // if it's an input type
    else if (element.type) {

        // if checkbox or radio, convert to boolean and apply to checked property
        if (element.type === 'checkbox' || element.type === 'radio') {
            element.checked = (value.toLowerCase() === 'true');
        }
        // if number based, convert to number and set the value
        else if (element.type === 'range' || element.type === 'number') {
            element.value = Number(value);
        }
        element.value = value;
    }
}
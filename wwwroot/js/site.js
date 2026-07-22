// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Calculator Logic
let calcCurrent = '';
const calcDisplay = document.getElementById('calcDisplay');

function calcAppend(val) {
    calcCurrent += val;
    calcUpdateDisplay();
}

function calcClear() {
    calcCurrent = '';
    calcUpdateDisplay();
}

function calcDelete() {
    calcCurrent = calcCurrent.slice(0, -1);
    calcUpdateDisplay();
}

function calcCalculate() {
    try {
        // Safe evaluation of basic math
        if (calcCurrent) {
            let result = new Function('return ' + calcCurrent)();
            if (result === undefined || isNaN(result) || !isFinite(result)) {
                calcCurrent = 'Hata';
            } else {
                // Limit to 4 decimal places
                calcCurrent = Math.round(result * 10000) / 10000 + '';
            }
        }
    } catch (e) {
        calcCurrent = 'Hata';
    }
    calcUpdateDisplay();
}

function calcUpdateDisplay() {
    if (calcDisplay) {
        calcDisplay.innerText = calcCurrent || '0';
    }
}

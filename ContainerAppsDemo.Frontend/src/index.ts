import './index.css';
import $ from 'cash-dom';

$(() => {
    $('#calculate').on('click', async () => {
        let a = parseInt($('#val-a').val() as string);
        let b = parseInt($('#val-b').val() as string);

        let response = await fetch(`/bff/calculate?a=${a}&b=${b}`);
        let result = await response.json();
        $('#result').text(`The result is ${result.result}`);
    });
});

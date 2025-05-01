document.getElementById('parse-btn').addEventListener('click', function() {
    var inputCode = document.getElementById('input').value;

    //Send MicroMl code to the server for parsing
    fetch('/parse', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({code: inputCode})
    })
    .then(response => response.json())
    .then(data => {
        //Display the AST result in the output section
        document.getElementById('ast-output').textContent = JSON.stringify(data,null, 2);
    })
    .catch(error => {
        console.error('Error parsing MicroML:', error);
    });
});
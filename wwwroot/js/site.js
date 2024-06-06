async function OnClickFav(label, url, buttonid) {
    var button = document.getElementById(buttonid);
    button.classList = ["color-button-onclick"];
    const response = await fetch('/Home/SaveFavouriteRecipe', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ label: label, url: url })
    });

    if (response.ok) {
        const result = await response.text();
        console.log(result); // handle the response
    } else {
        console.error('Error:', response.statusText);
    }
}
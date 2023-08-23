

function PostRequest(url = "", params = "", callback = null) {
    var http = new XMLHttpRequest();
    http.open('POST', url, true);

    //Send the proper header information along with the request
    http.setRequestHeader('Content-type', 'application/x-www-form-urlencoded');

    http.onreadystatechange = function () {//Call a function when the state changes.
        if (http.readyState == 4 && http.status == 200) {
            if (callback != null) {
                callback(http.responseText);
            }
        }
    }
    http.send(params);
}

var readResult = function (result) {

    const preview = document.getElementById("imagePreview");
	preview.src = result;
    console.log(result);
	var bytes = result.split("base64,")[1];

	var body = document.body;

    var inputelem = document.getElementById("picture");

	inputelem.value = bytes;
}

function handleImageFile(callback) {

	const file = document.querySelector('input[type=file]').files[0];
	const reader = new FileReader();

	reader.addEventListener("load", function () {
		callback(reader.result);
	}, false);

	if (file) {
		reader.readAsDataURL(file);
	}
}

function hideNewOrderCard() {
    var notificationCard = document.getElementById("newOrderCard");
    notificationCard.hidden = true;
}
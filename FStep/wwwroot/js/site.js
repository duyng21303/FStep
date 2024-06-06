// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function openForm() {
    document.getElementById("myForm").style.display = "block";
}

function closeForm() {
    document.getElementById("myForm").style.display = "none";
}

$(document).ready(function () {
    $('li').click(function () {
        var userId = $(this).data('userid');
        userID = userId;
        connection.invoke("LoadMessages", userId).catch(function (err) {
            return console.error(err.toString());
        });
        openForm();
    });
});

const connection = new signalR.HubConnectionBuilder().withUrl("/chathub").build();

connection.on("ReceiveMessage", function (user, message, timeSent) {
    const currentUser = '@User.Claims.FirstOrDefault(c => c.Type == "UserID")?.Value';
    const li = document.createElement('li');
    li.classList.add('clearfix');

    if (user === currentUser) {
        li.innerHTML = `
					<div class="message-data float-right">
						<span class="message-data-time">${timeSent}</span>
					</div>
					<br>
					<div class="message other-message float-right">${message}</div>
				`;
    } else {
        li.innerHTML = `
					<div class="message-data">
						<img src="https://bootdey.com/img/Content/avatar/avatar7.png" alt="avatar">
						<span class="message-data-time">${timeSent}</span>
					</div>
					<div class="message my-message">${message}</div>
				`;
    }

    document.getElementById('chatMessages').appendChild(li);
});

connection.on("LoadMessages", function (messages, currentUser) {
    const chatMessagesContainer = document.getElementById('chatMessages');
    chatMessagesContainer.innerHTML = ''; // Xóa các tin nhắn hiện tại
    console.log("Received messages:"); // Log messages to console

    messages.forEach(message => {
        const li = document.createElement('li');
        li.classList.add('clearfix');
        console.log("Check", currentUser); // Log messages to console

        if (message.senderUserId === currentUser) {

            li.innerHTML = `
							<div class="message-data float-right">
						<span class="message-data-time">${new Date(message.chatDate).toLocaleTimeString()}</span>
					</div>
					<br>
					<div class="message other-message float-right">${message.chatMsg}</div>
						`;
        } else {
            li.innerHTML = `
							<div class="message-data">
								<img src="https://bootdey.com/img/Content/avatar/avatar7.png" alt="avatar">
								<span class="message-data-time">${new Date(message.chatDate).toLocaleTimeString()}</span>
							</div>
							<div class="message my-message">${message.chatMsg}</div>
						`;
        }

        chatMessagesContainer.appendChild(li);
    });
});

connection.start().catch(function (err) {
    return console.error(err.toString());
});

document.getElementById('sendButton').addEventListener('click', function (event) {
    var message = document.getElementById('messageInput').value;
    var user = '@User.Claims.FirstOrDefault(c => c.Type == "UserID")?.Value';
    connection.invoke("SendMessage", userID, message).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});

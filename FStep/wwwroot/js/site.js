// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
// Lấy tất cả các nút cộng và trừ

function openForm() {
    document.getElementById("myForm").style.display = "block";
}

function closeForm() {
    document.getElementById("myForm").style.display = "none";
}

$(document).ready(function () {
    $('li').click(function () {
        var userId = $(this).data('userid');
        connection.invoke("LoadMessages", userId).catch(function (err) {
            return console.error(err.toString());
        });
        openForm();
    });
});

const connection = new signalR.HubConnectionBuilder().withUrl("/chathub").build();

connection.on("ReceiveMessage", function (recieveUser, user, message, img, date) {
    var currentUser = document.getElementById('sendUserID').value; // hoặc truyền từ server
    const li = document.createElement('li');
    li.classList.add('clearfix');
    console.log("check", user); // Log messages to console
    if (recieveUser === currentUser || user === currentUser) {
        if (user === currentUser) {
            li.innerHTML = `
					<div class="message-data float-right">
						<span class="message-data-time">${date}</span>
					</div>
					<br>
					<div class="message other-message float-right">${message}</div>
				`;
        } else {
            li.innerHTML = `
					<div class="message-data">
						<img src="/img/userAvar/${img}" alt="avatar">
						<span class="message-data-time">${date}</span>
					</div>
					<div class="message my-message">${message}</div>
				`;
        }
    }
    document.getElementById('chatMessages').appendChild(li);
    const chatHistory = document.getElementById('chat-history');
    chatHistory.scrollTop = chatHistory.scrollHeight;
});

connection.on("LoadMessages", function (messages, currentUser, recieverUser) {
    const chatMessagesContainer = document.getElementById('chatMessages');
    const chatMessagesSubmit = document.getElementById('submit-chat');
    const chatHistory = document.getElementById('chat-history');
    chatMessagesContainer.innerHTML = ''; // Xóa các tin nhắn hiện tại
    console.log("Received messages:", recieverUser); // Log messages to console
    const chatHeader = document.querySelector('.header-message');
    
    chatHeader.innerHTML = `
            <a href="javascript:void(0);" data-toggle="modal" data-target="#view_info">
										<img src="/img/userAvar/${recieverUser.avatarImg}" alt="avatar">
									</a>
									<div class="chat-about">
										<h7 class="m-b-0">${recieverUser.name}</h7><br>
									</div>
    `;
    messages.forEach(message => {
        const li = document.createElement('li');
        li.classList.add('clearfix');
        console.log("Check", currentUser); // Log messages to console
        if (message.chatMsg != null) {
            const formattedTime = new Date(message.chatDate).toLocaleTimeString('en-US', {
                hour: '2-digit',
                minute: '2-digit',
                hour12: false // Đặt giá trị này thành false để loại bỏ AM/PM
            });
            if (message.senderUserId === currentUser) {

                li.innerHTML = `
							<div class="message-data float-right">
						<span class="message-data-time">${formattedTime}</span>
					</div>
					<br>
					<div class="message other-message float-right">${message.chatMsg}</div>
						`;
            } else {
                li.innerHTML = `
							<div class="message-data">
								<img src="/img/userAvar/${recieverUser.avatarImg}" alt="avatar">
								<span class="message-data-time">${formattedTime}</span>
							</div>
							<div class="message my-message">${message.chatMsg}</div>
						`;
            }
        }
        chatMessagesContainer.appendChild(li);
    });
    chatMessagesSubmit.innerHTML = `<input type="text" class="form-control" placeholder="Enter text here..." id="messageInput">
								<div class="input-group-append">
							    <input type="hidden" id="recieveUserID" value="${recieverUser.idUser}" /> <!-- Giá trị của sendUserID -->
							    <input type="hidden" id="avatarimg" value="${recieverUser.avatarImg}" /> <!-- Giá trị của sendUserID -->
									<button class="btn btn-primary" id="sendButton">Send</button>
								`;

    document.getElementById('sendButton').addEventListener('click', function (event) {
        var message = document.getElementById('messageInput').value;
        if (message !== "") {
            var recieveUserID = document.getElementById('recieveUserID').value;
            var avatar = document.getElementById('avatarimg').value;
            var sendUserID = document.getElementById('sendUserID').value; // hoặc truyền từ server
            //console.log("Check", recieveUserID, sendUserID, message); // Log messages to console
            connection.invoke("SendMessage", recieveUserID, sendUserID, message, avatar).catch(function (err) {
                return console.error(err.toString());
            });
        }
        document.getElementById('messageInput').value = ''; // Clear the input field after sending the message
        event.preventDefault();
    });
    chatHistory.scrollTo({
        top: chatMessagesContainer.scrollHeight,
    });
});

connection.start().catch(function (err) {
    return console.error(err.toString());
});



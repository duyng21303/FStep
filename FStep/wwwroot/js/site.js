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
        var userId = $(this).data('userid').toString();
        $('li').css('background-color', '');

        // Set background color of the clicked list item
        $(this).css('background-color', '#f9f9f9');
        connection.invoke("LoadMessages", userId).catch(function (err) {
            return console.error(err.toString());
        });
        openForm();
    });
    $(".load-chat").click(function () {
        var userId = $(this).data("userid").toString();
        var post = $(this).data("post").toString();

        // Hiển thị chatContainer
        const chatContainer = document.getElementById('chatContainer');
        const chatContainerRaw = document.getElementById('chatContainerRaw');
        const chatContainerComment = document.getElementById('chatContainerComment');

        chatContainer.style.display = 'block';
        chatContainerRaw.innerHTML = '';

        //chatContainerRaw.style.zIndex = '1';
        // Thiết lập z-index cho chatContainer thấp hơn
        //chatContainer.style.zIndex = '2';
        // Gọi các hàm khác cần thiết (ví dụ: gọi connection.invoke và openForm)
        connection.invoke("LoadMessagesDetail", userId, post, "").catch(function (err) {
            return console.error(err.toString());
        });
        openForm();
    });
    $(".load-chat-comment").click(function () {
        var userId = $(this).data("userid").toString();
        var post = $(this).data("post").toString();
        var comment = $(this).data("comment").toString();

        // Hiển thị chatContainer
        const chatContainer = document.getElementById('chatContainerComment');
        const chatContainerRaw = document.getElementById('chatContainer');
        chatContainer.style.display = 'block';
        chatContainerRaw.innerHTML = '';
        //chatContainerRaw.style.zIndex = '1';
        // Thiết lập z-index cho chatContainer thấp hơn
        //chatContainer.style.zIndex = '2';
        // Gọi các hàm khác cần thiết (ví dụ: gọi connection.invoke và openForm)
        connection.invoke("LoadMessagesDetail", userId, post, comment).catch(function (err) {
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
    if (img != null) {
        img = "userAvar/" + img;
    } else {
        img = "nullAvar/149071.png";
    }
    console.log(confirm);
    const formattedTime = new Date(date).toLocaleTimeString('en-US', {
        day: '2-digit',
        month: '2-digit',
        hour: '2-digit',
        minute: '2-digit',
        hour12: false // Đặt giá trị này thành false để loại bỏ AM/PM
    });
    console.log("check", user); // Log messages to console
    if (recieveUser === currentUser || user === currentUser) {
        if (user === currentUser) {
            li.innerHTML = `
					<div class="message-data float-right">
						<span class="message-data-time">${formattedTime}</span>
					</div>
					<br>
					<div class="message other-message float-right">${message}</div>
				`;
        } else {
            li.innerHTML = `
					<div class="message-data">
						<img src="/img/${img}" alt="avatar">
						<span class="message-data-time">${formattedTime}</span>
					</div>
					<div class="message my-message">${message}</div>
				`;
        }
    }
    document.getElementById('chatMessages').appendChild(li);
    const chatHistory = document.getElementById('chat-history');
    chatHistory.scrollTop = chatHistory.scrollHeight;
});

connection.on("LoadMessages", function (messages, currentUser, recieverUser, confirm) {
    const chatMessagesContainer = document.getElementById('chatMessages');
    const chatMessagesSubmit = document.getElementById('submit-chat');
    const chatHistory = document.getElementById('chat-history');
    const confirmExchange = document.querySelector('.confirm-exchange');
    chatMessagesContainer.innerHTML = ''; // Xóa các tin nhắn hiện tại
    const chatHeader = document.querySelector('.header-message');
    var img;
    if (recieverUser.avatarImg != null) {
        img = "userAvar/" + recieverUser.avatarImg;
    } else {
        img = "nullAvar/149071.png";
    }
    chatHeader.innerHTML = `
        <a href="javascript:void(0);" data-toggle="modal" data-target="#view_info">
            <img src="/img/${img}" alt="avatar">
        </a>
        <div class="chat-about">
            <h7 class="m-b-0">${recieverUser.name}</h7><br>
        </div>
    `;
    if (confirm) {
        var comment = confirm.comment != null ? confirm.comment.idComment : "";
        console.log(comment)
        var accept = "";
        if (!confirm.checkConfirm) {
            accept = `
                <div class="col-6 text-end px-0">
                <button id="acceptButton" class="btn btn-success btn-sm equal-width"
                    style="font-size: 12px; padding: 3px 6px; margin: 0 0 4px; outline: none; box-shadow: none;"
                    onclick="accept()" data-userid="${recieverUser.idUser}" 
                    data-idpost="${confirm.post.idPost}" data-idcomment="${comment}">
                    Đồng ý
                </button>
                </div>
                <div class="col-6 text-begin px-1">
                <button id="declineButton" class="btn btn-danger btn-sm mb-1 equal-width" 
                    style="font-size: 12px; padding: 3px 6px; outline: none; box-shadow: none; background-color: grey; color: white;" 
                    onclick="decline()" data-userid="${recieverUser.idUser}" 
                    data-idpost="${confirm.post.idPost}" data-idcomment="${comment}" disabled>
                    Không đồng ý
                </button>
                </div>
                `;

        } else {
            accept = `
                <div class="col-6 text-end px-0">
                <button id="acceptButton" class="btn btn-success btn-sm mb-1 equal-width" 
                    style="font-size: 12px; padding: 3px 6px; outline: none; box-shadow: none; background-color: grey; color: white;" 
                    onclick="accept()" data-userid="${recieverUser.idUser}" 
                    data-idpost="${confirm.post.idPost}" data-idcomment="${comment}" disabled>
                    Đồng ý
                </button>
                </div>
                <div class="col-6 text-begin px-1">
                <button id="declineButton" class="btn btn-danger btn-sm equal-width" 
                    style="font-size: 12px; padding: 3px 6px; margin: 0 0 4px; outline: none; box-shadow: none;" 
                    onclick="decline()" data-userid="${recieverUser.idUser}" 
                    data-idpost="${confirm.post.idPost}" data-idcomment="${comment}">
                    Không đồng ý
                </button>
                </div>
                `;
        }
        confirmExchange.innerHTML = `
<div class="row">
        <!-- Phần chính chiếm 8 phần -->
        <div class="col-md-8">
            <div class="d-flex align-items-center">
                <div class="col-2 text-center p-0">
                    <!-- Hình ảnh sản phẩm nhỏ hơn -->
                    <img src="/img/postPic/${confirm.post.img}" alt="Product Image" class="img-thumbnail" style="width: 50px; height: 50px;">
                </div>
                <div class="col text-left p-2">
                    <!-- Tên sản phẩm và chi tiết, giảm kích thước chữ -->
                    <h6 class="product-name m-0" style="overflow: hidden; text-overflow: ellipsis; white-space: nowrap; max-width: 200px;"">${confirm.post.content}</h6>
                    <p class="product-detail m-0" style="font-size: 12px; overflow: hidden; text-overflow: ellipsis; white-space: nowrap; max-width: 200px;">
                    ${confirm.post.content}
                    </p>
                </div>
                <div class="col-2 text-center p-0">
                    <!-- Đặt chỗ trống để căn giữa nút đồng ý và không đồng ý -->
                </div>
            </div>
             <div class="row justify-content-around mt-2">
                    <!-- Nút đồng ý và không đồng ý, giảm kích thước -->
                    ${accept}
            </div>
        </div>
        <!-- Phần hehe chiếm 4 phần -->
        <div class="col-md-4" style="border-left: 1px solid #ccc;">
           
        </div>
    </div>
            `;
    } else {
        confirmExchange.innerHTML = '';
    }

    console.log(confirm ? confirm : "Post content is null or undefined");

    messages.forEach(message => {
        const li = document.createElement('li');
        li.classList.add('clearfix');
        console.log("Check", currentUser); // Log messages to console
        if (message.chatMsg != null) {
            const formattedTime = new Date(message.chatDate).toLocaleTimeString('en-US', {
                day: '2-digit',
                month: '2-digit',
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
                        <img src="/img/${img}" alt="avatar">
                        <span class="message-data-time">${formattedTime}</span>
                    </div>
                    <div class="message my-message">${message.chatMsg}</div>
                `;
            }
        }
        chatMessagesContainer.appendChild(li);
    });

    chatMessagesSubmit.innerHTML = `
        <input type="text" class="form-control" placeholder="Enter text here..." id="messageInput">
        <div class="input-group-append">
            <input type="hidden" id="recieveUserID" value="${recieverUser.idUser}" />
            <input type="hidden" id="avatarimg" value="${recieverUser.avatarImg}" />
            <button class="btn btn-primary" id="sendButton">Send</button>
        </div>
    `;

    document.getElementById('sendButton').addEventListener('click', function (event) {
        var message = document.getElementById('messageInput').value;
        if (message !== "") {
            var recieveUserID = document.getElementById('recieveUserID').value;
            var avatar = document.getElementById('avatarimg').value;
            var sendUserID = document.getElementById('sendUserID').value; // hoặc truyền từ server
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

// Hàm để kiểm tra trạng thái kết nối và thực hiện gọi invoke
async function safeInvoke(methodName, ...args) {
    if (connection.state === signalR.HubConnectionState.Connected) {
        try {
            await connection.invoke(methodName, ...args);
        } catch (err) {
            console.error(err.toString());
        }
    } else {
        console.error("Cannot send data if the connection is not in the 'Connected' State.");
    }
}
function accept() {
    const acceptButton = document.getElementById('acceptButton');
    const declineButton = document.getElementById('declineButton');
    // Chuyển nút "Đồng ý" sang trạng thái bấm không được và màu xám
    acceptButton.disabled = true;
    acceptButton.style.backgroundColor = 'grey';
    acceptButton.style.color = 'white';
    // Chuyển nút "Không đồng ý" sang trạng thái bấm được và màu gốc
    declineButton.disabled = false;
    declineButton.style.backgroundColor = '';
    declineButton.style.color = '';
    const userId = acceptButton.dataset.userid;
    const postId = acceptButton.dataset.idpost;
    const commentId = acceptButton.dataset.idcomment;
    const message = "Người dùng đã chấp nhận";
    connection.invoke("HandleAccept", message, userId, postId, commentId)
        .catch(function (err) {
            return console.error(err.toString());
        });

    displayAlert(message);
}

function decline() {
    const acceptButton = document.getElementById('acceptButton');
    const declineButton = document.getElementById('declineButton');
    // Chuyển nút "Không đồng ý" sang trạng thái bấm không được và màu xám
    declineButton.disabled = true;
    declineButton.style.backgroundColor = 'grey';
    declineButton.style.color = 'white';

    // Chuyển nút "Đồng ý" sang trạng thái bấm được và màu gốc
    acceptButton.disabled = false;
    acceptButton.style.backgroundColor = '';
    acceptButton.style.color = '';
    const userId = acceptButton.dataset.userid;
    const postId = acceptButton.dataset.idpost;
    const commentId = acceptButton.dataset.idcomment;
    const message = "Người dùng đã từ chối";
    connection.invoke("HandleDecline", message, userId, postId, commentId)
        .catch(function (err) {
            return console.error(err.toString());
        });

    displayAlert(message);
}

function displayAlert(message) {
    const alertContainer = document.getElementById('alert-container');
    const alertDiv = document.createElement('div');
    alertDiv.className = 'alert alert-info alert-dismissible fade show';
    alertDiv.role = 'alert';
    alertDiv.innerHTML = `
        ${message}
        <button type="button" class="close" data-dismiss="alert" aria-label="Close">
            <span aria-hidden="true">&times;</span>
        </button>
    `;
    alertContainer.appendChild(alertDiv);

    // Tự động ẩn alert sau 3 giây
    setTimeout(function () {
        alertDiv.classList.add('fade');
        setTimeout(function () {
            alertDiv.remove();
        }, 1000); // Thời gian 1 giây để ẩn điều chỉnh bên
    }, 3000); // Thời gian 3 giây để hiển thị
}

connection.start().catch(function (err) {
    return console.error(err.toString());
});

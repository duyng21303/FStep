
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
        connection.invoke("UpdateUserTab", userId, currentTab).catch(function (err) {
            return console.error(err.toString());
        });
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
    $('.load-chat-comment').click(function () {
        var userId = $(this).data('userid').toString();
        const chatContainer = document.getElementById('chatContainerComment');
        const chatContainerRaw = document.getElementById('chatContainer');
        const chatContainerRaw2 = document.getElementById('chatContainerRaw');
        chatContainer.style.display = 'block';
        chatContainerRaw.innerHTML = '';
        chatContainerRaw2.innerHTML = '';
        connection.invoke("LoadMessages", userId).catch(function (err) {
            return console.error(err.toString());
        });
        openForm();
    });
    $(".load-chat-comment-exchange").click(function () {
        var userId = $(this).data("userid").toString();
        var post = $(this).data("post").toString();
        var comment = $(this).data("comment").toString();

        // Hiển thị chatContainer
        const chatContainer = document.getElementById('chatContainerComment');
        const chatContainerRaw = document.getElementById('chatContainer');
        const chatContainerRaw2 = document.getElementById('chatContainerRaw');
        chatContainer.style.display = 'block';
        chatContainerRaw.innerHTML = '';
        chatContainerRaw2.innerHTML = '';
        //chatContainerRaw.style.zIndex = '1';
        // Thiết lập z-index cho chatContainer thấp hơn
        //chatContainer.style.zIndex = '2';
        // Gọi các hàm khác cần thiết (ví dụ: gọi connection.invoke và openForm)
        connection.invoke("LoadMessagesDetail", userId, post, comment).catch(function (err) {
            return console.error(err.toString());
        });
        openForm();
    });
    $(".notification-toggle").click(function () {
        _notiConnection.invoke("LoadNotification").catch(function (err) {
            return console.error(err.toString());
        });
    });
    $(".submitExchange").click(function () {

        const formData = new FormData(document.getElementById('exchangeForm'));
        const userId = $(this).data("userid");

        $.ajax({
            url: '/Post/CreateAnonymousExchange', // Update this URL as per your action and controller
            type: 'POST',
            data: formData,
            contentType: false,
            processData: false,
            success: function (response) {
                connection.invoke("SendExchange", userId, response.message)
                    .catch(function (err) {
                        console.error(err.toString());
                    });
                alert("Đã gửi trao đổi");
            },
            error: function (err) {
                console.error("Error: ", err);
                alert("Đã có lỗi xảy ra. Vui lòng thử lại.");
            }
        });
    });
});

const connection = new signalR.HubConnectionBuilder().withUrl("/chathub").build();
const _notiConnection = new signalR.HubConnectionBuilder().withUrl("/notificationhub").build();
_notiConnection.on("LoadNotification", function (notification) {
    const notificationList = document.getElementById('notification');
    notificationList.innerHTML = '';

    notification.forEach(notification => {
        var img = '';
        console.log(notification);
        if (notification.avatarImg != null) {
            img = "userAvar/" + recieverUser.avatarImg;
        } else {
            img = "nullAvar/149071.png";
        }
        const notificationItem = document.createElement('div');
        notificationItem.classList.add('notification-container');
        notificationItem.innerHTML = `
							<div class="notification-media">
								<img src="/img/${img}" alt="" class="notification-user-avatar">
							</div>
							<div class="notification-content">
								<p class="notification-text" style="max-height: 70px; overflow: hidden; transition: max-height 0.5s ease; text-overflow: ellipsis;">
												${notification.content}
								</p>
								<span class="notification-timer">${notification.date}</span>
							</div>
        `;
        notificationList.appendChild(notificationItem);
    });
});

function removeNotification(element) {
    element.parentElement.parentElement.parentElement.remove();
}

// Start the connection.

connection.on("ReceiveMessage", function (recieveUser, user, message, img, date) {
    var currentUser = document.getElementById('sendUserID').value; // hoặc truyền từ server
    const li = document.createElement('li');
    li.classList.add('clearfix');
    if (img != null) {
        img = "userAvar/" + img;
    } else {
        img = "nullAvar/149071.png";
    }
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
    const detailExchange = document.getElementById('detail-exchange');
    var img;
    detailExchange.innerHTML = '';
    if (recieverUser.avatarImg != null) {
        img = "userAvar/" + recieverUser.avatarImg;
    } else {
        img = "nullAvar/149071.png";
    }
    console.log(confirm)
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
        console.log(confirm)
        var accept = "";
        var currentStatus = `<span class="labelConfirm waiting">Đang chờ</span>`;

        var orderStatus = `<span class="labelConfirm waiting">Đang chờ</span>`;
        var removeConfirm = ``;
        var checkComment = confirm.comment != null ? true : false;
        if (!confirm.checkConfirm) {
            accept = `
                <div class="col-6 text-end px-0">
                <button id="acceptButton" class="btn btn-success btn-sm equal-width"
                    style="font-size: 12px; padding: 3px 6px; margin: 0 0 4px; outline: none; box-shadow: none;"
                    onclick="accept(${checkComment})" data-userid="${recieverUser.idUser}" 
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
        if (confirm.checkConfirm != null) {
            currentStatus = confirm.checkConfirm ? `<span class="labelConfirm accept">Đã chấp nhận</span>` :
                `<span class="labelConfirm decline">Không chấp nhận</span>`;
        }
        if (confirm.checkConfirmOrder != null) {
            orderStatus = confirm.checkConfirmOrder ? `<span class="labelConfirm accept">Đã chấp nhận</span>` :
                `<span class="labelConfirm decline">Không chấp nhận</span>`;
            if (confirm.checkConfirmOrder && confirm.checkConfirm == null) {
                accept = `
                <div class="col-6 text-end px-0">
                <button id="acceptButton" class="btn btn-success btn-sm mb-1 equal-width"
                    style="font-size: 12px; padding: 3px 6px; outline: none; box-shadow: none;"
                    onclick="accept()" data-userid="${recieverUser.idUser}" 
                    data-idpost="${confirm.post.idPost}" data-idcomment="${comment}" >
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
                `
            }
            if (confirm.checkConfirm != null) {
                if (!confirm.checkConfirm) {
                    removeConfirm = `
                     <p>Bạn có muốn hủy cuộc trao đổi này?
                     <a href="javascript:void(0);" onclick="deleteExchange();" data-idcurrent="${currentUser}" data-idrecieve="${recieverUser.idUser}" data-postid="${confirm.post.idPost}" class="delete-link mt-0">Hủy trao đổi</a></p>
                    `
                }
            }
        }
        if (confirm.comment != null) {
            if (confirm.comment.idUser == currentUser) {
                detailExchange.innerHTML = `
        <div class="container">
    <div class="exchange-container" style="max-height: 500px; overflow-y: auto;">
        <!-- Your Item -->
        <div class="exchange-item text-center" style="padding: 10px; border: 1px solid #ddd; border-radius: 5px;">
            <h5>Bạn</h5>
            <img src="/img/postPic/${confirm.comment.img}" alt="Your Item" class="item-image" style="max-width: 100%; height: auto; margin-top: 10px;">
            <p>${confirm.comment.content}</p>
        </div>

        <!-- Exchange Icon -->
        <div class="d-flex align-items-center justify-content-center">
            <i class="fa fa-exchange-alt exchange-icon" aria-hidden="true"></i>
        </div>

        <!-- Other Person's Item -->
        <div class="exchange-item text-center" style="max-height: 500px; overflow-y: auto; padding: 10px; border: 1px solid #ddd; border-radius: 5px;">
            <div class="user-info">
                <img src="/img/${img}" alt="Other User" class="user-image" style="width: 50px; height: 50px; border-radius: 50%;">
                <div>
                    <h5>${recieverUser.name}</h5>
                </div>
            </div>
            <img src="/img/postPic/${confirm.post.img}" alt="Other Item" class="item-image" style="max-width: 100%; height: auto; margin-top: 10px;">
            <p>${confirm.post.detail}</p>
        </div>
    </div>
</div>

        `;
            } else {
                detailExchange.innerHTML = `
        <div class="container">
    <div class="exchange-container">
        <!-- Your Item -->
        <div class="exchange-item text-center" style="max-height: 500px; overflow-y: auto; padding: 10px; border: 1px solid #ddd; border-radius: 5px;">
            <h5>Bạn</h5>
            <img src="/img/postPic/${confirm.post.img}" alt="Your Item" class="item-image" style="max-width: 100%; height: auto; margin-top: 10px;">
            <p>${confirm.post.detail}</p>
        </div>

        <!-- Exchange Icon -->
        <div class="d-flex align-items-center justify-content-center">
            <i class="fa fa-exchange-alt exchange-icon" aria-hidden="true"></i>
        </div>

        <!-- Other Person's Item -->
        <div class="exchange-item text-center" style="max-height: 500px; overflow-y: auto; padding: 10px; border: 1px solid #ddd; border-radius: 5px;">
            <div class="user-info">
                <img src="/img/${img}" alt="Other User" class="user-image" style="width: 50px; height: 50px; border-radius: 50%;">
                <div>
                    <h5>${recieverUser.name}</h5>
                </div>
            </div>
            <div class="item-content" style="margin-top: 10px;">
                <img src="/img/postPic/${confirm.comment.img}" alt="Other Item" class="item-image" style="max-width: 100%; height: auto; margin-top: 10px;">
                <p>${confirm.comment.content}</p>
            </div>
        </div>
    </div>
</div>

        `;
            }

        } else {
            detailExchange.innerHTML = `
            <div id="tab-comment-image" class="tab-pane fade show p-0">
    <div class="inner-column wow fadeInRight card" style="padding: 5px; box-shadow: 0 3px 10px rgb(0 0 0 / 0.2);">
        <span class="lead">Sản phẩm trao đổi</span>
        <form method="post" id="exchangeForm" enctype="multipart/form-data">
            <div>
                <input name="IdPost" value="${confirm.post.idPost}" type="hidden" />
                <textarea rows="3" class="form-control" name="Content"></textarea>
            </div>
            <div style="min-height:10px;"></div>
            <div>
                <div class="form-item">
                    <label class="form-label my-3">Thêm hình ảnh</label><sup>*</sup>
                    <input type="file" name="img" class="form-control">
                </div>
            </div>
            <div style="min-height:10px;"></div>
            <div>
                <button id="submitExchange" class="btn btn-danger btn-sm equal-width submitExchange"
                        style="font-size: 12px; padding: 3px 6px; margin: 0 0 4px; outline: none; box-shadow: none;"
                        type="submit" onclick="submitExchange()">
                    Tạo thông tin trao đổi
                </button>
            </div>
        </form>
    </div>
</div>
             `;
        }

        confirmExchange.innerHTML = `
<div class="row">
        <!-- Phần chính chiếm 8 phần -->
        <div class="col-md-7">
            <div class="d-flex align-items-center" data-bs-toggle="modal" data-bs-target="#exchangeModal" style="cursor: pointer;">
                <div class="col-2 text-center p-0">
                    <!-- Hình ảnh sản phẩm nhỏ hơn -->
                    <img src="/img/postPic/${confirm.post.img}" alt="Product Image" class="img-thumbnail" style="width: 50px; height: 50px; min-height: 50px; min-width: 50px">
                </div>
                <div class="col text-left p-2 ml-12">

                    <!-- Tên sản phẩm và chi tiết, giảm kích thước chữ -->
                    <h6 class="product-name m-0" style="overflow: hidden; text-overflow: ellipsis; white-space: nowrap; max-width: 180px; max-height: 20px;"">${confirm.post.content}</h6>
                    <div class="product-detail m-0" style="font-size: 12px; overflow: hidden; text-overflow: ellipsis; white-space: nowrap; max-width: 180px; max-height: 20px;">
                    ${confirm.post.detail}
                    </div>
                </div>
            </div>
             <div class="row justify-content-around mt-2">
                    <!-- Nút đồng ý và không đồng ý, giảm kích thước -->
                    ${accept}
            </div>
                <div class="content-confirm p-1">
                    <!-- Nút đồng ý và không đồng ý, giảm kích thước -->
                    ${removeConfirm}
                </div>
        </div>
        <!-- Phần hehe chiếm 4 phần -->
        <div class="col-md-5" style="border-left: 1px solid #ccc;">
        <div class="content-confirm">
            <p>
            Tình trạng trao đổi của bạn: 
            </p>
            ${currentStatus}
            <p>
            Tình trạng trao đổi của ${recieverUser.name}:
            </p>
            <div class="pb-1">
            ${orderStatus}
            </div>
        </div>
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
                    <div class="message-data float-left">
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
    const exchangeForm = document.getElementById('exchangeForm');
    if (exchangeForm) {
        exchangeForm.addEventListener('submit', function (event) {
            event.preventDefault(); // Ngăn chặn hành vi mặc định của form

            // Reset previous validation messages
            exchangeForm.classList.remove('was-invalidated');

            const content = exchangeForm.querySelector('textarea[name="Content"]');
            const img = exchangeForm.querySelector('input[name="img"]');
            let isValid = true;

            if (!content.value.trim()) {
                content.classList.add('is-invalid');
                isValid = false;
            } else {
                content.classList.remove('is-invalid');
            }

            if (!img.files.length) {
                img.classList.add('is-invalid');
                isValid = false;
            } else {
                img.classList.remove('is-invalid');
            }

            if (!isValid) {
                exchangeForm.classList.add('was-invalidated');
                return;
            }

            // Lấy dữ liệu từ form
            const formData = new FormData(exchangeForm);

            // Thực hiện yêu cầu gửi form qua Ajax 
            fetch('/Post/CreateAnonymousExchage', {
                method: 'POST',
                body: formData
            })
                .then(response => {
                    if (!response.ok) {
                        throw new Error('Network response was not ok');
                    }
                    return response.json();
                })
                .then(data => {
                    // Xử lý phản hồi từ server
                    if (data.success) {
                        displayAlert("Bạn đã tạo thành công bài viết trao đổi");
                        const exchangeModal = document.getElementById('exchangeModal');
                        if (exchangeModal) {
                            const modal = bootstrap.Modal.getInstance(exchangeModal);
                            modal.hide();
                            exchangeForm.reset();
                        }
                        connection.invoke("LoadMessagesDetail", recieverUser.idUser.toString(), data.idPost.toString(), data.commentID.toString()).catch(function (err) {
                            return console.error(err.toString());
                        });
                    } else {
                        exchangeForm.reset();
                        alert('Form submission failed: ' + data.message);
                    }
                })
                .catch(error => {
                    console.error('Error:', error);
                    alert('An error occurred while submitting the form. Please try again.');
                });
        });
    }
    chatHistory.scrollTo({
        top: chatMessagesContainer.scrollHeight,
    });

});

// Hàm để kiểm tra trạng thái kết nối và thực hiện gọi invoke

function accept() {
    const acceptButton = document.getElementById('acceptButton');
    const declineButton = document.getElementById('declineButton');
    const userId = acceptButton.dataset.userid;
    const postId = acceptButton.dataset.idpost;
    const commentId = acceptButton.dataset.idcomment;
    // Chuyển nút "Đồng ý" sang trạng thái bấm không được và màu xám
    if (commentId != "") {
        acceptButton.disabled = true;
        acceptButton.style.backgroundColor = 'grey';
        acceptButton.style.color = 'white';
        // Chuyển nút "Không đồng ý" sang trạng thái bấm được và màu gốc
        declineButton.disabled = false;
        declineButton.style.backgroundColor = '';
        declineButton.style.color = '';
        const message = "Người dùng đã chấp nhận";
        connection.invoke("HandleAccept", message, userId, postId, commentId)
            .catch(function (err) {
                return console.error(err.toString());
            });
        displayAlert(message);
    }
    else {
        modal = new bootstrap.Modal(document.getElementById('exchangeModal'), {
            KeyboardEvent: false
        });
        modal.show();
    }
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
function deleteExchange() {
    var currentUser = $(".delete-link").data("idcurrent").toString();
    var recieve = $(".delete-link").data("idrecieve").toString();
    var post = $(".delete-link").data("postid").toString();
    connection.invoke("DeleteExchange", currentUser, recieve, post)
        .catch(function (err) {
            return console.error(err.toString());
        });
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
function CreateNotification(userID, typeMessage, type, parameter, idEvent) {
    _notiConnection.invoke("CreateNotification", userID, typeMessage, type, parameter, idEvent)
        .catch(function (err) {
            return console.error(err.toString());
        });
}

connection.start().catch(function (err) {
    return console.error(err.toString());
});
_notiConnection.start().catch(function (err) {
    return console.error(err.toString());
});
//$(document).ready(function () {
//    $('#createPostForm').submit(function (event) {
//        event.preventDefault(); // Prevent the default form submission
//        var formData = new FormData(this); // Get the form data

//        $.ajax({
//            type: 'POST',
//            url: $(this).attr('action'),
//            data: formData,
//            processData: false,
//            contentType: false,
//            success: function (response) {
//                if (response.success) {
//                    // Handle success (e.g., close modal, update UI)
//                    $('#exampleModal').modal('hide');
//                    window.location.reload(); // Optionally reload the page to reflect changes
//                } else {
//                    // Display error message
//                    alert(response.message);
//                }
//            },
//            error: function (response) {
//                // Handle error
//                alert('An error occurred. Please try again.');
//            }
//        });
//    });
//});
//const connection = new signalR.HubConnectionBuilder()
//    .withUrl("/exchangeHub")
//    .build();

//connection.start().catch(function (err) {
//    return console.error(err.toString());
//});

//function submitExchange() {
//    const formData = new FormData(document.getElementById('exchangeForm'));
//    const userId = $("#submitExchange").data("userid");

//    $.ajax({
//        url: '/YourController/YourAction', // Thay đổi URL theo action và controller của bạn
//        type: 'POST',
//        data: formData,
//        contentType: false,
//        processData: false,
//        success: function (response) {
//            connection.invoke("SendExchange", userId, response.message).catch(function (err) {
//                return console.error(err.toString());
//            });
//            alert("Đã gửi trao đổi");
//        },
//        error: function (err) {
//            console.error("Error: ", err);
//        }
//    });
//}
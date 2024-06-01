// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
// Lấy tất cả các nút cộng và trừ
const minusBtn = document.querySelector(".qty-btn.minus");
const plusBtn = document.querySelector(".qty-btn.plus");

// Lấy input
const inputQty = document.querySelector(".pro-qty input");

// Thêm sự kiện khi click vào nút trừ
minusBtn.addEventListener("click", function () {
    let value = parseInt(inputQty.value);
    if (value > 1) {
        inputQty.value = value - 1;
    }
});

// Thêm sự kiện khi click vào nút cộng
plusBtn.addEventListener("click", function () {
    let value = parseInt(inputQty.value);
    inputQty.value = value + 1;
});
//comment
document.querySelector('.button__cmt-send').addEventListener('click', function () {
    var commentText = document.querySelector('.textarea').value;

    fetch('/your-controller/SubmitComment', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify({ commentText: commentText }),
    })
        .then(response => {
           
        })
        .catch(error => {
            
        });
});



let childPic = document.getElementsByClassName("child-product-pic");

let activeImg = document.getElementsByClassName("child-product-pic active");

for (var i = 0; i < childPic.length; i++)
{
	childPic[i].addEventListener("click", function ()
	{
		if (activeImg.length > 0)
		{
			activeImg[0].classList.remove("active");
		}
		this.classList.add("active");
		document.getElementById("bigImage").src = this.src;
		imageZoom("bigImage");
	});
}

let arrowLeft = document.getElementById("slideLeft");
let arrowRight = document.getElementById("slideRight");

arrowLeft.addEventListener("mousedown", function ()
{
	var t = document.getElementsByClassName("slider");
	t[0].scrollLeft -= 100;
});
arrowRight.addEventListener("mousedown", function ()
{
	document.getElementsByClassName("slider")[0].scrollLeft += 100;
});


function imageZoom(imageId)
{
	let imgContainer = document.getElementsByClassName("product-big-image-container")[0];
	let img = document.getElementById(imageId);
	let lens = document.getElementById("lens");
	lens.style.backgroundImage = `url(${img.src})`;
	let ratio = 3;
	lens.style.backgroundSize = (img.width * ratio) + "px " + (img.height * ratio) + "px";
	lens.style.backgroundRepeat = "no-repeat";
	img.addEventListener("mousemove", moveLens);
	img.addEventListener("touchmove", moveLens);
	lens.addEventListener("mousemove", moveLens);
	lens.addEventListener("touchmove", moveLens);

	function moveLens()
	{
		let pos = getCursor();
		let zoomImgPosTop = img.offsetTop + (pos.y) - (lens.offsetHeight / 2);
		let zoomImgposLeft = img.offsetLeft + (pos.x) - (lens.offsetWidth / 2);
		if (zoomImgPosTop < -lens.offsetHeight/2)
			zoomImgPosTop = -lens.offsetHeight/2;
		else if (zoomImgPosTop > img.offsetTop + img.offsetHeight - (lens.offsetHeight / 2))
			zoomImgPosTop = img.offsetTop + img.offsetHeight - (lens.offsetHeight / 2);
		if (zoomImgposLeft < -lens.offsetWidth/2)
			zoomImgposLeft = -lens.offsetWidth/2;
		else if (zoomImgposLeft > img.offsetLeft + img.offsetWidth - (lens.offsetHeight / 2))
			zoomImgposLeft = img.offsetLeft + img.offsetWidth - (lens.offsetHeight / 2);
		if (pos.x <= 0)
			pos.x = 0;
		if (pos.y <= 0)
			pos.y = 0;
		lens.style.top = zoomImgPosTop + "px";
		lens.style.left = zoomImgposLeft + "px";
		lens.style.backgroundPosition =  -1*(((pos.x * ratio) - (lens.offsetWidth / 2))) + "px " + (-1*(((pos.y * ratio) - (lens.offsetHeight / 2)))) + "px";
	}

	function getCursor()
	{
		let e = window.event;
		let bounds = img.getBoundingClientRect();
		let x = e.pageX - (bounds.left + window.pageXOffset);
		let y = e.pageY - (bounds.top + window.pageYOffset);
		return { x, y };
	}
}
imageZoom("bigImage");

var dataFromServer;
var readDataFromServer = 0;
var commentPageSize = 2;
$(window).scroll(function ()
{
	if (!document.getElementById("tab-3").classList.contains("active"))
		return;
	if ($(window).scrollTop() == $(document).height() - $(window).height())
	{
		if ((dataFromServer != null) && dataFromServer.length <= readDataFromServer)
		{
			return;
		}

		else if ((dataFromServer != null) && dataFromServer.length > readDataFromServer)
		{
			document.getElementById("tab-3").innerHTML += showTreeForList();
			return;
		}

		let productId = document.URL.split("/")[4];
		$.ajax({
			type: "GET",
			url: "/Home/GetComments/",
			data: "productId=" + productId,
			success: function (data)
			{
				dataFromServer = data;
				if(data.length != 0)
					document.getElementById("tab-3").innerHTML += showTreeForList();
			},
			error: function (data)
			{
				swal("خطایی پیش آمده", "در بارگذاری نظرات خطایی پیش آمده لطفا بعدا مجددا تلاش نمایید", "warning");
			}
		});

		function showTreeForList()
		{
			var treeText = "";
			for (var i = readDataFromServer; i < commentPageSize; i++, readDataFromServer++)
			{
				treeText += showTree(dataFromServer[i]);
			}
			commentPageSize *= 2;
			if (commentPageSize > dataFromServer.length)
				commentPageSize = dataFromServer.length;
			return treeText;
		}
		function showTree(comment)
		{
			var text = `<div class="row comment comment${comment.replyCommentId}" style="margin-right:${comment.depth * 4}%; ${comment.replyCommentId != null ? "display:none" : ""}">
							<p style="display:none">${comment.userId}</p>
							<h3>${comment.userFullName != null ? comment.userFullName : "کاربر فروشگاه"}
								<b>${new Date(comment.dateTime).toLocaleString("fa-IR")}</b></h3>
							<p>
								${comment.text}
							</p>
							<button class="btn btn-small ${isLiked(comment) == true ? "btn-success" : ""}" onClick="likeComment(${comment.id},this)"><i class="fa fa-thumbs-up"></i></button>
							<button class="btn btn-small btn-info" onClick="replyComment(${comment.id},${comment.depth},this)">پاسخ دادن به این نظر</button>
							${comment.childComments.length == 0 ? "" : '<button class="btn btn-small btn-warning" onClick="toggleReplies(\'comment' + comment.id + '\',this)">نمایش پاسخ ها</button>'}
						</div>`;
			function isLiked(comment)
			{
				for (var i = 0; i < comment.commentLikes.length; i++)
				{
					if (comment.commentLikes[i].commentId == comment.id)
						return true;
				}
				return false;
			}
			if (comment.childComments.length < 1)
			{
				return text;
			}
			for (var i = 0; i < comment.childComments.length; i++)
			{
				text += showTree(comment.childComments[i]);
			}
			return text;
		}
	}
});
function likeComment(commentId, btn)
{
	$.ajax({
		type: "GET",
		url: "/Home/LikeComment/",
		data: "commentId=" + commentId,
		success: function (data)
		{
			swal("عملیات انجام شد", data.message, "success");
			if (btn.classList.contains("btn-success"))
				btn.classList.remove("btn-success");
			else
				btn.classList.add("btn-success");
		},
		error: function (data)
		{
			swal("عملیات با خطا مواجه شد", data.responseJSON.message, "error");
		}
	});
}
function replyComment(commentId, commentDepth, replybtn)
{
	let productId = document.URL.split("/")[4];
	let t = replybtn.parentElement;
	t.innerHTML += `<div class="row comments style="margin-right:${commentDepth * 4}%;">
							<input type="text" name="text" class="form-control" placeholder="جواب کامنت رو بنویس"/>
							<button onClick="addComment(${commentId},this.parentNode)" class="form-control btn btn-success btn-block">ثبت شود </button>
					</div>`
}
function toggleReplies(replyCommentId, btnElement)
{
	let hiddenComments = document.getElementsByClassName(replyCommentId);
	for (var i = 0; i < hiddenComments.length; i++)
	{
		if (hiddenComments[i].style.display == "none")
		{
			hiddenComments[i].style.display = "block";
			btnElement.innerHTML = "پنهان کردن پاسخ ها";
		}
		else
		{
			hiddenComments[i].style.display = "none";
			btnElement.innerHTML = "نمایش پاسخ ها";
		}
	}
}
function addComment(replyCommentId, element)
{
	let productId = document.URL.split("/")[4];
	let input = element.getElementsByTagName("input")[0];
	const comment = {
		productId: productId,
		ReplyCommentId: replyCommentId,
		text: input.value
	}
	$.ajax({
		type: "GET",
		url: "/Home/AddComment/",
		data: comment,
		dataType: "json",
		crossOrigin: true,
		contentType: 'application/json',
		success: function (data)
		{
			swal("عملیات انجام شد", data.message, "success");
			let text = `
							<p style="display:none">${data.data.userId}</p>
							<h3>${data.data.userFullName != null ? data.data.userFullName : "کاربر فروشگاه"}
								<b>${new Date(data.data.dateTime).toLocaleString("fa-IR")}</b></h3>
							<p>
								${data.data.text}
							</p>
							<button class="btn btn-small" onClick="likeComment(${data.data.id},this)"><i class="fa fa-thumbs-up"></i></button>
							<button class="btn btn-small btn-info" onClick="replyComment(${data.data.id},${data.data.depth},this)">پاسخ دادن به این نظر</button>
							${data.data.childComments.length == 0 ? "" : '<button class="btn btn-small btn-warning" onClick="toggleReplies(\'comment' + data.data.id + '\',this)">نمایش پاسخ ها</button>'}
						`;
			const newComment = document.createElement("div");
			newComment.classList.add("row","comment", `comment${data.data.replyCommentId}`);
			newComment.style.marginRight = data.data.depth * 4 + "%";
			newComment.innerHTML = text;
			if (data.data.replyCommentId)
			{
				element.parentElement.insertAdjacentElement("afterend", newComment);
			}
			else
			{
				element.insertAdjacentElement("afterend", newComment);
			}
		},
		error: function (data)
		{
			swal("عملیات با خطا مواجه شد", data.responseJSON.message, "error");
		}
	});
}
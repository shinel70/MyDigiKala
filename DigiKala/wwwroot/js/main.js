

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
		if (zoomImgPosTop < 0)
			zoomImgPosTop = 0;
		else if (zoomImgPosTop > img.offsetTop + img.offsetHeight - (lens.offsetHeight / 2))
			zoomImgPosTop = img.offsetTop + img.offsetHeight - (lens.offsetHeight / 2);
		if (zoomImgposLeft < 0)
			zoomImgposLeft = 0;
		else if (zoomImgposLeft > img.offsetLeft + img.offsetWidth - (lens.offsetHeight / 2))
			zoomImgposLeft = img.offsetLeft + img.offsetWidth - (lens.offsetHeight / 2);

		lens.style.top = zoomImgPosTop + "px";
		lens.style.left = zoomImgposLeft + "px";
		lens.style.backgroundPosition = "-" + ((pos.x * ratio) - (lens.offsetWidth / 2)) + "px -" + ((pos.y * ratio) - (lens.offsetWidth / 2)) + "px";
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
				document.getElementById("tab-3").innerHTML += showTreeForList();
			},
			error: function (data)
			{
				window.alert("مشکلی پیش اومده");
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
							<h3>${comment.userFullName != null ? comment.user.fullName : "کاربر فروشگاه"}
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
			window.alert(data);
			if (btn.classList.contains("btn-success"))
				btn.classList.remove("btn-success");
			else
				btn.classList.add("btn-success");
		},
		error: function (data)
		{
			window.alert(data.responseJSON.data);
		}
	});
}
function replyComment(commentId, commentDepth, replybtn)
{
	let productId = document.URL.split("/")[4];
	let t = replybtn.parentElement;
	let f = replybtn.parentNode;
	t.innerHTML += `<div class="row comments style="margin-right:${commentDepth * 4}%;">
						<form action="/home/AddComment">
							<input type="hidden" name="productId" value="${productId}" />
							<input type="hidden" name="replyCommentId" value="${commentId}" />	
							<input type="hidden" name="depth" value="${commentDepth+1}" />
							<input type="text" name="text" class="form-control" placeholder="جواب کامنت رو بنویس"/>
							<input type="submit" class="form-control btn btn-success btn-block" value="ثبت شود"/>
						</form>
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
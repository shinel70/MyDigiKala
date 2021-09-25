function refreshViewBasket()
{
	try
	{
		let itemsInBasket = JSON.parse(localStorage.getItem("basket"));
		let basket = document.getElementById("basket");
		let text = `<tr>
						<th>
							عکس
						</th>
						<th>
							نام کالا
						</th>
						<th>
							قیمت واحد
						</th>
						<th>
							تعداد
						</th>
						<th>
							قیمت کل
						</th>
					</tr>`;
		if (itemsInBasket == undefined || itemsInBasket.length == 0)
		{
			basket.innerHTML = text;
			document.getElementById("basketCount").innerHTML = "";
		}
		else
		{
			let len = itemsInBasket.length;
			let totalPrice = 0;
			let productIdQties = [];
			for (let i = 0; i < len; i++)
			{
				productIdQties.push({ "id": itemsInBasket[i].id, "qty": itemsInBasket[i].qty });
				let totalItemPrice = (itemsInBasket[i].price * itemsInBasket[i].qty);
				totalPrice += totalItemPrice;
				text += `<tr id="${itemsInBasket[i].id}">
						<td><img style="max-width:25px;max-height:25px;" src="/images/products/${itemsInBasket[i].img}" /></td>
						<td>${itemsInBasket[i].name}</td>
						<td>${itemsInBasket[i].price.toLocaleString('fa-IR', { minimumFractionDigits: 0, maximumFrationDigit: 0 })} ریال</td>
						<td style="white-space:nowrap;">
							<i onClick="addQty(this.parentNode)" class="fa fa-plus-circle text-success" style="cursor:pointer;"></i>
							<span id="qty">${itemsInBasket[i].qty}</span>
							<i onClick="minusQty(this.parentNode)" class="fa fa-minus-circle text-danger" style="cursor:pointer;"></i>
						</td>
						<td>
							<span id="qtySum">${totalItemPrice.toLocaleString('fa-IR', {
					minimumFractionDigits: 0,
					maximumFrationDigit: 0
							})} ریال</span>
						</td>
					</tr>`
			}

			text += `<tr><td colspan="4"><a class="btn btn-danger btn-block" href='/home/AddOrder?productIdQties=${JSON.stringify(productIdQties)}'>ثبت خرید</a></td><td>${totalPrice.toLocaleString('fa-IR', { minimumFractionDigits: 0, maximumFrationDigit: 0 })}ریال</td></tr>`;
		}
		basket.innerHTML = text;
		let foo = itemsInBasket.reduce((total, obj) => total.qty += obj.qty);
		if (itemsInBasket.length == 1)
			document.getElementById("basketCount").innerHTML = foo.qty;
		else
			document.getElementById("basketCount").innerHTML = foo;
	}
	catch (e)
	{
		localStorage.removeItem("basket");
	}
}
function addToBasket(obj)
{
	try
	{
		let itemsInBasket = JSON.parse(localStorage.getItem("basket"));
		if (itemsInBasket == undefined || itemsInBasket.length == 0)
		{
			itemsInBasket = [];
			itemsInBasket.push(obj);
			localStorage.setItem("basket", JSON.stringify(itemsInBasket));
		}
		else
		{
			let existItem = itemsInBasket.find((i) => i.id == obj.id);
			if (existItem)
			{
				existItem.qty++;
				localStorage.setItem("basket", JSON.stringify(itemsInBasket));
			}
			else
			{
				itemsInBasket.push(obj);
				localStorage.setItem("basket", JSON.stringify(itemsInBasket));
			}
		}
	}
	catch (e)
	{
		localStorage.removeItem("basket");
	}
	finally
	{
		refreshViewBasket();
	}
}
function addQty(tdQty)
{
	let itemsInBasket = JSON.parse(localStorage.getItem("basket"));
	let item = itemsInBasket.find((i) => i.id == tdQty.parentNode.id);
	item.qty++;
	localStorage.setItem("basket", JSON.stringify(itemsInBasket));
	refreshViewBasket();
}
function minusQty(tdQty)
{
	let itemsInBasket = JSON.parse(localStorage.getItem("basket"));
	let itemIndex = itemsInBasket.findIndex((i) => i.id == tdQty.parentNode.id);
	itemsInBasket[itemIndex].qty--;
	if (itemsInBasket[itemIndex].qty < 1)
	{
		itemsInBasket.splice(itemIndex, 1);
	}
	localStorage.setItem("basket", JSON.stringify(itemsInBasket));
	refreshViewBasket();
}

refreshViewBasket();

function MyBanners()
{
	$.ajax({
		url: "/Home/Banners/",
		type: "Get",
		data: {}
	}).done(function (result)
	{
		$('#myModal').modal('show');
		$('#bodyModal').html(result);
	});
}
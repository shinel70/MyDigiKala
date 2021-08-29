    var swiper = new Swiper('.zoom-container', {
      slidesPerView: 4,
     
      slidesPerGroup: 4,
      loop: true,
      loopFillGroupWithBlank: true,
      pagination: {
        el: '.swiper-dots',
        clickable: true,
      },
		
      
    });



// mobile Slider

  var swiper2 = new Swiper('.mobile-slider', {
      pagination: {
        el: '.mobile-pagination',
      },
    });

// Products- Slider

    var swiper3 = new Swiper('.products-slider', {
      navigation: {
        nextEl: '.swiper-button-next',
        prevEl: '.swiper-button-prev',
      },
		
			breakpoints: {
        640: {
          slidesPerView: 2,
           
        },
        768: {
          slidesPerView: 3,
           
        },
			 992: {
          slidesPerView: 4,
           
        },
        1200: {
          slidesPerView: 5,
           
        },
      }
    });

//pictures zoom 

$(".zoom").elevateZoom();


// Pictures onclick
  function show(pic) {
            // document.getElementsByTagName("img")[pic].classList.add("d-none");
            $(".big-product-img img").addClass("display-none");
            document.getElementsByTagName("img")[pic].classList.remove("display-none");
        }
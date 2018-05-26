/// <reference path="jquery-2.2.1-vsdoc.js" />

/*put this codes inside function after { start mark:
 /// <summary>
 ///    summery about method
 /// </summary>
 /// <param name="inputName" type="inputType">
 ///   ablout input
 /// </param>
 /// <param name="input2Name" type="input2Type">
 ///   about input2
 /// </param>
 /// <returns type="retyrnType" />
 */


var SRL = function () {

};
window.SRL = SRL;

SRL.AccessCheck = function (cookieName, cookieValue) {
    if (ReadCookie(cookieName) != cookieValue) $('.' + cookieValue).hide();
};


SRL.AvoidDropdownClose = function (selector, event) {
    $(selector).on(event + ".bs.dropdown", function (e) {
        e.stopPropagation();
        e.preventDefault();
    });
};


SRL.BindBigCityToOstan = function (stateID, bigCityId, optionClasses) {
    $('#' + stateID).on('change', function (e) {
        StartLoader('#' + bigCityId + '-icon', '#' + bigCityId + '-loader');
        $('#' + bigCityId).empty().append('<option value="">انتخاب کنید</option>');
        var optionSelected = $("option:selected", this);
        var valueSelected = this.value;
        if (valueSelected) {
            $('#' + bigCityId).attr('disabled', false);
            RenderSelectOption('GetBigCities', true, { stateID: valueSelected }, $('#' + bigCityId), optionClasses);
        }
        else
            $('#' + bigCityId).attr('disabled', true);
        $('#' + bigCityId).val(null);
        EndLoader('#' + bigCityId + '-icon', '#' + bigCityId + '-loader');
    });
};

SRL.BodyAnimatePaddingTop = function (height) {
    //var height = $('#main_nav').height();
    $('body').animate({ paddingTop: height });
};

SRL.CheckCheckBox = function (isValid, target) {
    if (target.prop('checked')) {
        $("#" + target.attr("id") + "Response").text("");
        isValid.valid *= 1;
    }
    else {
        $("#" + target.attr("id") + "Response").text("اعلام موافقت با قوانین اجباری است");
        isValid.valid *= 0;
    }
};

SRL.CheckNationalCode = function (isValid, target) {
    if (target.val()) {
        if (CheckNationalCodeDigit(target.val())) {
            target.css("border", "");
            $("#" + target.attr("id") + "Response").text("");
            isValid.valid *= 1;
        }
        else {
            target.css("border", "2px solid red");
            $("#" + target.attr("id") + "Response").text(target.attr("placeholder"));
            isValid.valid *= 0;
        }
    }
};


SRL.CheckNationalCodeDigit = function (nationalCode) {
    var L = nationalCode.length;
    if (L < 8 || parseInt(nationalCode, 10) == 0) return 0;
    nationalCode = ('0000' + nationalCode).substr(L + 4 - 10);
    if (parseInt(nationalCode.substr(3, 6), 10) == 0) return 0;
    var c = parseInt(nationalCode.substr(9, 1), 10);
    var s = 0;
    for (var i = 0; i < 9; i++)
        s += parseInt(nationalCode.substr(i, 1), 10) * (10 - i);
    s = s % 11;
    x = (s < 2 && c == s) || (s >= 2 && c == (11 - s));
    if (x) x = 1;
    else x = 0;
    return x;
    return 1;
};


SRL.CheckUsernameUnique = function (isValid, target) {
    var data = { username: target.val() };
    var dataAsJson = JSON.stringify(data);
    ServerRequest(true, isValid, dataAsJson, true, 'CheckUsernameUnique', null, null, function (result) {
        if (result == true) {
            target.css("border", "");
            $("#" + target.attr("id") + "Response").text("");
            isValid.valid *= 1;
        }
        else if (result == false) {
            target.css("border", "2px solid red");
            $("#" + target.attr("id") + "Response").text(target.attr("placeholder"));
            isValid.valid *= 0;
        }
        else {
            alert("server error- CheckUsernameUnique");
            isValid.valid *= 0;
        }
    });
};


SRL.CheckFieldNotNull = function (isValid, target) {
    if (!target.val() && target.attr('required') == 'required') {
        target.css("border", "2px solid red");
        isValid.valid *= 0;
        return 0;
    }
    else {
        target.css("border", "");
        isValid.valid *= 1;
        return 1;
    }
};


SRL.CheckSelect2NotNull = function (isValid, target) {
    var targetVal = ($.isArray(target.val())) ? target.val()[0] : target.val();
    if (!targetVal && target.attr('required') == 'required') {
        $("#" + target.attr("id") + "Response").text(target.attr("placeholder"));
        isValid.valid *= 0;
        return 0;
    }
    else {
        $("#" + target.attr("id") + "Response").text("");
        isValid.valid *= 1;
        return 1;
    }
};


SRL.CheckButtonHasValue = function (isValid, target) {
    if (!target.val() && target.hasClass('required')) {
        target.css("border", "1px solid red");
        isValid.valid *= 0;
        return 0;
    }
    else {
        target.css("border", "");
        isValid.valid *= 1;
        return 1;
    }
};


SRL.CheckFieldPattern = function (isValid, target) {

    var pattern = new RegExp(target.attr("pattern"));
    if (!pattern.test(target.val())) {
        target.css("border", "2px solid red");
        $("#" + target.attr("id") + "Response").text(target.attr("placeholder"));
        isValid.valid *= 0;
    }
    else {
        target.css("border", "");
        $("#" + target.attr("id") + "Response").text("");
        isValid.valid *= 1;
    }
};


SRL.CheckConfPasswordField = function (isValid, target) {
    if (target.val() != $("#password").val()) {
        target.css("border", "2px solid red");
        $("#" + target.attr("id") + "Response").text(target.attr("placeholder"));
        isValid.valid *= 0;
    }
    else {
        target.css("border", "");
        $("#" + target.attr("id") + "Response").text("");
        isValid.valid *= 1;
    }
};


SRL.CreateTree = function (treeId, data, showTag) {
    $('#' + treeId).treeview({
        data: data,
        backColor: '#99d6ff',
        levels: 2,
        showTags: showTag
    });
};


SRL.CreateCookie = function (name, value, days) {
    var expires;

    if (days) {
        var date = new Date();
        date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
        expires = "; expires=" + date.toGMTString();
    } else {
        expires = "";
    }
    document.cookie = encodeURIComponent(name) + "=" + encodeURIComponent(value) + expires + "; path=/";
};

SRL.DropdownHover = function (isSmooth, dropdownSelector, fadeInDelay, fadeOutDelay) {
    /// <summary>
    ///     1: makes dropdown auto open smoothly
    ///     &#10;    1.1 - $(bool, selector)
    ///     &#10;    1.6 - $()
    ///     &#10;2: dropdown auto open in block
    ///     &#10;    2.1 - $(html, ownerDocument)
    ///     &#10;    2.2 - $(html, attributes)
    /// </summary>
    /// <param name="isSmooth" type="bool">
    ///    smoothly or block
    /// </param>
    /// <param name="dropdownSelector" type="cssSelector">
    ///    css selector
    /// </param>
    /// <returns type="void" />

    var dropdownMenu = jQuery(dropdownSelector).find('.dropdown-menu');
    if (isSmooth) {
        jQuery(dropdownSelector).hover(function () {
            dropdownMenu.stop(true, true).delay(fadeInDelay).fadeIn();
        }, function () {
            dropdownMenu.stop(true, true).delay(fadeOutDelay).fadeOut();
        });
    }
    else {
        jQuery(dropdownSelector).hover(function () {
            dropdownMenu.css("display", "block");
        }, function () {
            dropdownMenu.css("display", "none");
        });

    }
};

SRL.EraseCookie = function (name) {
    CreateCookie(name, "", -1);
};


SRL.EnableModalClosing = function (page) {
    $(page).click(function (e) {
        if (!$(e.target).hasClass("dropLink") && !$(e.target).closest('.loginModal').length) {
            var dropdowns = $(".dropdown-content");
            for (var i = 0; i < dropdowns.length; i++) {
                var openDropdown = dropdowns[i];
                if (openDropdown.classList.contains('show')) {
                    openDropdown.classList.remove('show');
                }
            }
        }
    });
};


SRL.EndLoader = function (toHideSelector, loaderSelector, enableSelector) {//
    if (toHideSelector) $(toHideSelector).show();
    if (loaderSelector) $(loaderSelector).css('display', 'none');
    if (enableSelector) $(disableSelector).attr('disabled', false);
};


SRL.FocusOutValidation = function (selector) {//FocusOutValidation(".register input");
    $(selector).each(
        function (i) {
            var target = $(this);
            target.focusout(function () {
                FieldValidation(target, true);
            });
        }
    );
};


SRL.FocusOutValidationSelect2 = function (selector) {//FocusOutValidation(".sel2");
    var isValid = { valid: 1 };
    $(selector).each(
        function (i) {
            var target = $(this);
            target.on('select2-blur', function () {
                CheckSelect2NotNull(isValid, target);
            });
            target.on('select2-removed', function () {
                CheckSelect2NotNull(isValid, target);
            });
        }
    );
};


SRL.FieldValidation = function (target, async) {
    var isValid = { valid: 1 };
    var inputId = target.attr("id");
    switch (inputId) {
        case "username":
            if (CheckFieldNotNull(isValid, target))
                if (async) CheckUsernameUnique(isValid, target);
            break;
        case "usernameLogin":
        case "passwordLogin":
            CheckFieldNotNull(isValid, target);
            break;
        case "email":
        case "mobile":
        case "password":
            CheckFieldPattern(isValid, target);
            break;
        case "confPassword":
            CheckConfPasswordField(isValid, target);
            break;
        case "nationalCode":
            CheckNationalCode(isValid, target);
            break;
        case "confirmLaw":
            CheckCheckBox(isValid, target);
            break;
        case "newAdCategory":
        case "newAdBtnIcon":
            CheckButtonHasValue(isValid, target);
            break;
        default:
            CheckFieldNotNull(isValid, target);
    }
    var isSelect2 = target.hasClass('sel2');
    if (isSelect2) CheckSelect2NotNull(isValid, target);
    return isValid.valid;
};


SRL.FormValidation = function (selector) {
    var isValid = 1;
    $(selector).each(
        function (i) {
            var target = $(this);
            isValid *= FieldValidation(target, false);
        }
    );
    return isValid;
};


SRL.FileUploader_ashx = function (asynchronous, isValid, formData, action, beforeCallBack, completeCallBack, successCallBack) {
    formData.append('action', action);
    var url = "../FileUploader.ashx";
    $.ajax({
        async: asynchronous,
        type: "post",
        data: formData,
        url: url,
        contentType: false,
        processData: false,
        dataType: 'json',
        beforeSend: function (jqXHR) {
            if (beforeCallBack) beforeCallBack();
        },
        success: function (data, textStatus, jqXHR) {
            if (data) {
                if (data.redirect) Redirect(data.redirect, data.redirectData);
                else successCallBack(data);
            }
            else successCallBack(data);
        },
        error: function (response, x, y) {
            alert(response.responseText);
            if (isValid) isValid.valid *= 0;
        },
        complete: function (jqXHR, textStatus) {
            if (completeCallBack) completeCallBack();
        }
    });
};

SRL.FlexSlider = function (selector, animationType, speed, hoverPause, isSlide) {
    $(selector).flexslider({
        animation: animationType,
        slideshowSpeed: speed,
        pauseOnHover: hoverPause,
        slideshow: isSlide,
        directionNav: false,
        pauseOnAction: false,
    });
};

SRL.GetUrlParameter = function (uri, key) {
    var uriParamLine = decodeURIComponent(uri.search.substring(1));
    var params = uriParamLine.split('&');
    var keyValue, keyParam, i;
    for (i = 0; i < params.length; i++) {
        keyValue = params[i].split('=');
        keyParam = keyValue[0];
        if (keyParam === key) {
            var valueParam = keyValue[1];
            var value = valueParam === undefined ? true : valueParam;
            return value;
        }
    }
};


SRL.GetTagData = function (tagSelector, dataName) {
    var data = $(tagSelector).data(dataName);
    return data;
};

SRL.Inview = function (viewTarget, isOneTime, visibleFunction, invisibleFunction) { // ('.about-bg-heading', true,function(){})
    if (isOneTime) {
        $(viewTarget).one('inview', function (event, visible) {
            if (visible == true) {
                visibleFunction();
            }
            else if (visible == false && invisibleFunction) {
                invisibleFunction();
            }
        });
    }
    else {
        $(viewTarget).on('inview', function (event, visible) {
            if (visible == true) {
                visibleFunction();
            }
            else if (visible == false && invisibleFunction) {
                invisibleFunction();
            }
        });

    }
};

SRL.ListActiveChange = function (liSelector) {// changes li active class on clicking. liSelector end to li like: ListActiveChange(".nav li")
    $(liSelector).on("click", function () {
        $(liSelector).removeClass("active");
        $(this).addClass("active");
    });
};

SRL.LoaderSpinNG = function (moduleName, attrName,spinerFile) {
    moduleName.directive(attrName, ['$http', function ($http) {
        return {
            restrict: "A",
            template: '<div><img src="{{spinerFileFnG}}"/></div>',
            link: function (scope, elm, attrs) {
                scope.spinerFileFnG = spinerFile;
                scope.isLoading = function () {
                    return $http.pendingRequests.length > 0;
                };
                scope.$watch(scope.isLoading, function (v) {
                    if (v) {
                        elm.show();
                    } else {
                        elm.hide();
                    }
                });
            }
        };
    }]);
};

SRL.LoadPage = function (loaderId, page) {
    $('#' + loaderId).load(page);
};

SRL.LoadScriptInline = function (scriptUrl, document_, callback, callbackParamsObj) {

    var script = document_.createElement("script")
    script.type = "text/javascript";

    if (script.readyState) {  //IE
        script.onreadystatechange = function () {
            if (script.readyState == "loaded" ||
                    script.readyState == "complete") {
                script.onreadystatechange = null;
                callback(callbackParamsObj);
            }
        };
    } else {  //Others
        script.onload = function () {
            callback(callbackParamsObj);
        };
    }

    script.src = scriptUrl;
    document.getElementsByTagName("head")[0].appendChild(script);
};

SRL.LoadSelectOption = function (list, selectTarget, optionClasses) {
    var optionTag = '<option ';
    if (optionClasses) optionTag += 'class="' + optionClasses + '" ';

    $(selectTarget).empty().append(optionTag + 'value="">انتخاب کنید</option>');
    for (var i in list) {
        var name = list[i].name;
        var id = list[i].id;
        var item = $(optionTag + ">").attr("value", id).text(name);
        selectTarget.append(item).attr('disabled', false);
    }
};


SRL.LoadHtmlInDiv = function (divId, htmlPage) {
    $("#" + divId).load(htmlPage);
};


SRL.LoadUsernameInPage = function (id) {
    var target = $('#' + id);
    var username = ReadCookie('username');
    var showText = "";
    if (username) showText += username;
    showText += " خوش آمدید";
    target.text(showText);
};


SRL.LoadUserAddressForAdd = function (chBTarget, ostanId, bigCityId, addressConId) {
    if ($(chBTarget).prop('checked')) {
        ServerRequest(true, null, null, true, 'GetUserAddressData', null, null, function (result) {
            $('#' + ostanId).val(result.ostanId).attr('disabled', true);
            FieldValidation($('#' + ostanId), true);
            var bigCity = $("<option>").attr("value", result.bigCityId).text(result.bigCityName);
            $('#' + bigCityId).empty().append(bigCity).attr('disabled', true);
            FieldValidation($('#' + bigCityId), true);
            $('#' + addressConId).val(result.addressContinue);
        });
    }
    else {
        $('#' + ostanId).val(null).attr('disabled', false);
        $('#' + bigCityId).empty().append('<option value="">انتخاب کنید</option>').attr('disabled', true);
        $('#' + addressConId).val(null);
    }
};


SRL.LoadJsCssFile = function (filename, filetype) {
    if (filetype == "js") { //if filename is a external JavaScript file
        // alert('called');
        var fileref = document.createElement('script');
        fileref.setAttribute("src", filename);
        alert('called');
    }
    else if (filetype == "css") { //if filename is an external CSS file
        var fileref = document.createElement("link");
        fileref.setAttribute("rel", "stylesheet");
        fileref.setAttribute("type", "text/css");
        fileref.setAttribute("href", filename);
    }
    if (typeof fileref != "undefined") {
        document.getElementsByTagName("head")[0].appendChild(fileref);
        alert(document.getElementsByTagName("head")[0].innerHTML);
    }
};


SRL.MakeObjectData = function (classSelector) {
    var data = new Object();
    $(classSelector).each(
        function (i) {
            var target = $(this);
            data[target.attr("id")] = ($.isArray(target.val())) ? target.val()[0] : target.val();
        }
    );
    return data;
};


SRL.MakeFormData = function (formData, selector) {
    $.each($(selector), function (i, target) {
        formData.append($(target).attr("id"), $(target).val());
    });
    return formData;
};


SRL.MakeImageData = function (formData, imageId) {
    var images = $('#' + imageId).prop('files');
    $.each(images, function (i, image) {
        formData.append(imageId + i, image);
    });
    return formData;
};


SRL.MapCreateHomeControl = function (point, controlDiv, map) {
    controlDiv.style.padding = '5px';
    var controlUI = document.createElement('div');
    controlUI.style.backgroundColor = 'yellow';
    controlUI.style.border = '1px solid';
    controlUI.style.cursor = 'pointer';
    controlUI.style.textAlign = 'center';
    controlUI.title = 'انتقال به مرکز';
    controlDiv.appendChild(controlUI);
    var controlText = document.createElement('div');
    controlText.style.fontFamily = 'Arial,sans-serif';
    controlText.style.fontSize = '12px';
    controlText.style.paddingLeft = '4px';
    controlText.style.paddingRight = '4px';
    controlText.innerHTML = '<b>مرکز تهران<b>'
    controlUI.appendChild(controlText);

    // Setup click-event listener: simply set the map to point
    google.maps.event.addDomListener(controlUI, 'click', function () {
        map.setCenter(point)
    });
    map.controls[google.maps.ControlPosition.TOP_RIGHT].push(controlDiv);
};


SRL.MapGoogleRender = function (mapId, centerLat, centerLon, zoom, mapPropNum, hasDefaultMarker, hasMarkerInfo, infoText, radius, markerZoom, hasHome) {
    //AIzaSyCgLiaA30V7dBrNUmOt1LQdjnC2H18nv7k

    // MapGoogleRender('mapDoctors', 35.6875572374694, 51.38837814331055, 12, 1);
    //var tehranCenter = new google.maps.LatLng(35.6875572374694, 51.38837814331055);
    var center = new google.maps.LatLng(centerLat, centerLon);
    var mapProp1 = {
        center: center,
        zoom: zoom,
        panControl: true,
        zoomControl: true,
        mapTypeControl: true,
        scaleControl: true,
        streetViewControl: true,
        scrollwheel: false,
        overviewMapControl: true,
        rotateControl: true,
        mapTypeId: google.maps.MapTypeId.ROADMAP
    };
    var mapProp;
    switch (mapPropNum) {
        case 1:
            mapProp = mapProp1;
            break;
    }
    var map = new google.maps.Map(document.getElementById(mapId), mapProp);
    google.maps.event.addListener(map, 'click', function (event) {
        if (typeof (hasMarkerInfo) === "boolean") { //only false or true works for hasMarkerInfo
            var marker = MapPlaceMarker(event.latLng, map, hasMarkerInfo, infoText);
            if (markerZoom) MapZoomOnMarkerClick(map, marker, markerZoom);
        }
        if (radius) MapCircle(event.latLng, map, radius);
    });
    if (hasHome) MapCreateHomeControl(center, document.createElement('div'), map);
    if (hasDefaultMarker) SRL.MapPlaceMarker(center, map);
    return { center: center, map: map };
};


SRL.MapActivityArea = function (mapId) {
    var tehranCenter = new google.maps.LatLng(35.6875572374694, 51.38837814331055);
    var mapProp = {
        center: tehranCenter,
        zoom: 12,
        panControl: true,
        zoomControl: true,
        mapTypeControl: true,
        scaleControl: true,
        streetViewControl: true,
        overviewMapControl: true,
        rotateControl: true,
        mapTypeId: google.maps.MapTypeId.ROADMAP
    };
    var map = new google.maps.Map(document.getElementById(mapId), mapProp);
    google.maps.event.addListener(map, 'click', function (event) {
        var marker = MapPlaceMarker(event.latLng, map, true, 'محدوده فعالیت انتخاب شد       ');
        MapCircle(event.latLng, map, 2000);
        MapZoomOnMarkerClick(map, marker, 14);
    });
    MapCreateHomeControl(tehranCenter, document.createElement('div'), map);
};


SRL.MapPlaceMarker = function (location, map, hasInfoWindow, infoText) {
    var marker = new google.maps.Marker({
        position: location, //var location= new google.maps.LatLng(51.508742,-0.120850);
        //animation: google.maps.Animation.BOUNCE,
        //icon: '/File/leaf-orange.png',
        map: map //var map=new google.maps.Map(document.getElementById("googleMap"), mapProp);
    });
    marker.setMap(map);
    var infowindow = new google.maps.InfoWindow({
        // content: 'Latitude: ' + location.lat() + '<br>Longitude: ' + location.lng()
        content: infoText
    });
    if (hasInfoWindow) infowindow.open(map, marker);
    return marker;
};


SRL.MapCircle = function (center, map, radius) {
    var circle = new google.maps.Circle({
        center: center,
        radius: radius,
        strokeColor: "#0000FF",
        strokeOpacity: 0.8,
        strokeWeight: 2,
        fillColor: "red",
        fillOpacity: 0.4
    });

    circle.setMap(map);
};


SRL.MapDirection = function (map) {
    var directionsService = new google.maps.DirectionsService;
    var directionsDisplay = new google.maps.DirectionsRenderer;

    directionsDisplay.setMap(map);

    directionsService.route({
        //origin: "chicago, il",
        origin: new google.maps.LatLng(48.7488628862, 1.0548108816),
        //destination: "st louis, mo",
        destination: new google.maps.LatLng(49.4988628862, 9.31548108816),
        travelMode: 'DRIVING'
    }, function (response, status) {
        if (status === 'OK') {
            directionsDisplay.setDirections(response);
        } else {
            window.alert('Directions request failed due to ' + status);
        }
    });
};


SRL.MapZoomOnMarkerClick = function (map, marker, zoom) {
    google.maps.event.addListener(marker, 'click', function () {
        map.setZoom(zoom);
        map.setCenter(marker.getPosition());
    });
};


SRL.MapAddressPoint = function (mapId) {
    var tehranCenter = new google.maps.LatLng(35.6875572374694, 51.38837814331055);
    var mapProp = {
        center: tehranCenter,
        zoom: 12,
        panControl: true,
        zoomControl: true,
        mapTypeControl: true,
        scaleControl: true,
        streetViewControl: true,
        overviewMapControl: true,
        rotateControl: true,
        mapTypeId: google.maps.MapTypeId.ROADMAP
    };
    var map = new google.maps.Map(document.getElementById(mapId), mapProp);

    google.maps.event.addListener(map, 'click', function (event) {
        var marker = MapPlaceMarker(event.latLng, map, false);
        MapZoomOnMarkerClick(map, marker, 14);
    });
    CreateHomeControl(tehranCenter, document.createElement('div'), map);
};

SRL.NumberCounter = function (target, speed, swingORlinear) {// (this,5000,'swing')
    //target is a jquery which points to block having number inside, e.g. target= $('#ss') in <span id='ss'>789</span>
    $(target).prop('Counter', 0).animate({
        Counter: $(target).text()
    }, {
        duration: speed,
        easing: swingORlinear,
        step: function (now) {//now is current Counter number in each animate step: a number between 0 and target number
            $(target).text(Math.ceil(now));
        }
    });
};

SRL.ProgressBar = function (barId, start, end, interval) {
    clearInterval(id);
    var elem = $("#" + barId);
    var width = start;
    var id = setInterval(Progresser, interval, end);//end is Progresser param
    function Progresser(finish) {
        if (width >= 100 || width >= finish) {
            clearInterval(id);
        } else {
            width++;
            elem.css("width", width + '%');
            $("#activationBarLabel").text(width * 1 + '%');
        }
    }

    return width;
};


SRL.RenderAffix = function (affixSelector, topHeight) {// ('#main-nav',100) or ('#main-nav' , $('header').height())
    $(affixSelector).affix({
        offset: {
            top: topHeight
        }
    });
};


SRL.RenderDashboardPage = function (username) {
    AccessCheck('role', 'admin');
    RequestAds("userAdsContainer");
    FocusOutValidation("#newAdForm input,#newAdForm select");
};


SRL.RenderRegisterPage = function () {
    FocusOutValidation('#register input');
    $('.register-option').on('click', function () {
        if (GetTagData($(this), 'selected') != true) {
            SetTagData($('.register-option'), 'selected', false);
            $('.register-option').css('background-color', 'rgb(245,245,245)');
            SetTagData($(this), 'selected', true);
            $(this).css('background-color', '#ff8080');
        }
        $('#register-btn').css('display', 'block');
        $('.register-field').css('display', 'none');
        $('#register-step2').css('display', 'none');
        if ($(this).attr('id') == 'option-company') {
            $('#register-company').css('display', 'block');
        }
        if ($(this).attr('id') == 'option-driver') {
            $('#register-driver').css('display', 'block');
        }
        if ($(this).attr('id') == 'option-requester') {
            $('#register-requester').css('display', 'block');
        }
        MapActivityArea('mapActivityArea');
        MapAddressPoint('mapCompanyAddress');
    });
};


SRL.RenderDatalist = function (method, dlTarget) { //RenderDatalist('GetStates', $("#statesList"));
    ServerRequest(true, null, null, true, method, null, null, function (result) {
        for (var i in result) {
            var name = result[i].name;
            var id = result[i].id;
            var item = $("<option>").attr("value", id).text(name);
            dlTarget.append(item);
        }
    });
};


SRL.RenderSelectOption = function (method, async, dataOrNull, selectTarget, optionClasses) {//RenderSelectOption('GetStates', null, $('#newAdOstan'), 'text-right rtl');
    var data = dataOrNull ? JSON.stringify(dataOrNull) : null;
    ServerRequest(async, null, data, true, method, null, null, function (result) {
        LoadSelectOption(result, selectTarget, optionClasses);
    });
};


SRL.RenderCategoryMgmt = function () {
    var categoryList = RequestCategoryList(false);
    RemoveAddCssClass('#categoryEditForm', 'show', 'hide');
    CreateTree('categoryMgmtTree', categoryList, false);
    $('#categoryMgmtTree').on('nodeSelected', function (event, node) {
        RemoveAddCssClass('#categoryEditForm', 'hide', 'show');
        var selectedNode = $('#categoryMgmtTree').treeview('getSelected');
        $('#tbCategoryMgmtNodeName').val(selectedNode[0].text);
    });
    $('#categoryMgmtTree').on('nodeUnselected ', function (event, node) {
        RemoveAddCssClass('#categoryEditForm', 'show', 'hide');
    });
};


SRL.RenderCarTypeMgmt = function () {

    RemoveAddCssClass('#carTypeEditForm', 'show', 'hide');
    $('select#carType').on('change', function (e) {
        var optionSelected = $("option:selected", this);
        var valueSelected = this.value;
        RemoveAddCssClass('#carTypeEditForm', 'hide', 'show');
        $('#tbCarType').val(valueSelected);
    });
};


SRL.RenderCategorySelector = function (treeId) {
    CreateTree(treeId, RequestCategoryList(false), false);
    RemoveAddCssClass('#btnDivCategorySelect', 'show', 'hide');
    AvoidDropdownClose('.dropdown-category .dropdown-body', 'click');
    $('#' + treeId).on('nodeSelected', function () {
        RemoveAddCssClass('#btnDivCategorySelect', 'hide', 'show');
    });
    $('#' + treeId).on('nodeUnselected ', function () {
        RemoveAddCssClass('#btnDivCategorySelect', 'show', 'hide');
    });
};

SRL.RenderEachPageJS = function (page) {// use this function in each project in projectJS.js with name RenderEachPage
    var pageDefalutName = page.location.pathname.split("/").pop();
    var pageName = pageDefalutName.toLowerCase();
    switch (pageName) {
        case "":
            //do something
            break;
    }

};

SRL.RenderNewAd = function () {
    RenderSelectOption('GetStates', true, null, $('#newAdOstan'));
    BindBigCityToOstan('newAdOstan', 'newAdBigCity');
    RenderSingleFileInput('newAdIconFile', 'newAdIconLabel', 'newAdBtnIcon', 'newAdIcon', 'newAddIconContainer', 'newAdIconClose');
    RenderMultiFileInput('newAdPicFiles', 3);
};

SRL.RenderPageJS = function (page) {// use this function in each project in projectJS.js with name RenderPage
    //EnableModalClosing(page);
    RenderEachPage(page);//hhhh
    //ServerRequest(true, null, '{"usernameLogin":"soheil","passwordLogin":"123"}', true, "TryLogin", null, null, function () { CreateCookie('username', ''); CreateCookie('role', 'admin'); });

};

SRL.RenderSingleAd = function (containerId, ad) {
    var str = '<div id="" class="adSingle">' +
        '<div id="" class="adContent">' +
        '<div id="" class="adTitle">' +
        '<a href="#" class="adTitleLink">' +
        ad.title +
        '</a>' +
        '</div>' +
        '<div id="" class="adSmallPic">' +
        // '<img src="../Files/ref.jpg" alt="Smiley face" class="smallPic">'+
        '</div>' +
        '<div id="" class="adPrice">قیمت: ' +
        ad.price +
        ' تومان، ' +
        ad.priceType +
        '</div>' +
        '<div id="" class="adAddress">' +
        ad.state +
        '، ' +
        ad.bigCity +
        '</div>' +
        '</div>' +
        '</div>';
    $('#' + containerId).prepend(str);
};


SRL.RenderSingleFileInput = function (inputFileId, labelId, btnFileId, iconId, iconContainer, iconClose) {
    $('#' + inputFileId).css({ 'visibility': 'hidden', 'position': 'absolute' });
    var lblTxt = $('#' + labelId).text();
    var btnTxt = $('#' + btnFileId).text();
    SetClickEvent(btnFileId, inputFileId);
    ResetFileInput(inputFileId, labelId, null, btnFileId, null, iconId, iconContainer, iconClose);
    $('#' + inputFileId).on('change', function (event) {
        ResetFileInput(null, labelId, lblTxt, btnFileId, 'انتخاب ایکون', iconId, iconContainer, iconClose);
        var file = event.target.files[0];
        if (file.size >= 262144) {
            $('#' + labelId).html('فایل انتخابی از حداکثر حجم مجاز ایکون (256 کیلوبایت) بیشتر است').css({ 'color': 'red' });
            FieldValidation($('#' + btnFileId), true);
            return;
        }
        var fileTempPath = URL.createObjectURL(file);
        var fileName = file.name;
        var filePath = $('#' + inputFileId).val();
        $('#' + labelId).html(fileName);
        $('#' + iconId).fadeIn("fast").attr('src', fileTempPath);
        $('#' + btnFileId).html('تغییر ایکون').val(filePath);
        RemoveAddCssClass('#' + iconClose + ', #' + iconContainer, 'hide', 'show');
        FieldValidation($('#' + btnFileId), true);
    });
    $('#' + iconClose).on('click', function () {
        ResetFileInput(inputFileId, labelId, lblTxt, btnFileId, 'انتخاب ایکون', iconId, iconContainer, iconClose);
        FieldValidation($('#' + btnFileId), true);
    });
};


SRL.RenderMultiFileInput = function (inputFileId, fileCount) {
    "use strict";
    $("#" + inputFileId).fileinput({
        fileSingle: 'فایل',
        filePlural: 'فایل',
        browseLabel: 'انتخاب &hellip;',
        removeLabel: 'حذف',
        removeTitle: 'پاکسازی فایل‌های انتخاب شده',
        cancelLabel: 'لغو',
        cancelTitle: 'لغو بارگزاری جاری',
        uploadLabel: 'بارگذاری',
        uploadTitle: 'بارگذاری فایل‌های انتخاب شده',
        maxFileCount: fileCount,
        msgNo: 'No',
        msgNoFilesSelected: '',
        msgCancelled: 'Cancelled',
        msgZoomModalHeading: 'Detailed Preview',
        msgSizeTooLarge: 'فایل "{name}" (<b>{size} کیلوبایت</b>) از حداکثر مجاز <b>{maxSize} کیلوبایت</b>.',
        msgFilesTooLess: 'شما باید حداقل <b>{n}</b> {files} فایل برای بارگذاری انتخاب کنید.',
        //  msgFilesTooMany: 'تعداد فایل‌های انتخاب شده برای بارگذاری <b>({n})</b> از حداکثر مجاز عبور کرده است <b>{m}</b>.',
        msgFilesTooMany: 'حداکثر تعداد فایل قابل انتخاب <b>({m})</b> فایل می باشد',
        msgFileNotFound: 'فایل "{name}" یافت نشد!',
        msgFileSecured: 'محدودیت های امنیتی مانع خواندن فایل "{name}" است.',
        msgFileNotReadable: 'فایل "{name}" قابل نوشتن نیست.',
        msgFilePreviewAborted: 'پیشنمایش فایل "{name}". شکست خورد',
        msgFilePreviewError: 'در هنگام خواندن فایل "{name}" خطایی رخ داد.',
        msgInvalidFileType: 'نوع فایل "{name}" معتبر نیست. فقط "{types}" پشیبانی می‌شود.',
        msgInvalidFileExtension: 'پسوند فایل "{name}" معتبر نیست. فقط "{extensions}" پشتیبانی می‌شود.',
        msgUploadAborted: 'The file upload was aborted',
        msgValidationError: 'خطای اعتبار سنجی',
        // msgLoading: 'بارگیری فایل {index} از {files} &hellip;',
        //msgProgress: 'بارگیری فایل {index} از {files} - {name} - {percent}% تمام شد.',
        msgSelected: '{n} {files} انتخاب شده',
        msgFoldersNotAllowed: 'فقط فایل‌ها را بکشید و رها کنید! {n} پوشه نادیده گرفته شد.',
        msgImageWidthSmall: 'عرض فایل تصویر "{name}" باید حداقل {size} پیکسل باشد.',
        msgImageHeightSmall: 'ارتفاع فایل تصویر "{name}" باید حداقل {size} پیکسل باشد.',
        msgImageWidthLarge: 'عرض فایل تصویر "{name}" نمیتواند از {size} پیکسل بیشتر باشد.',
        msgImageHeightLarge: 'ارتفاع فایل تصویر "{name}" نمی‌تواند از {size} پیکسل بیشتر باشد.',
        msgImageResizeError: 'یافت نشد ابعاد تصویر را برای تغییر اندازه.',
        msgImageResizeException: 'خطا در هنگام تغییر اندازه تصویر.<pre>{errors}</pre>',
        dropZoneTitle: 'فایل‌ها را بکشید و در اینجا رها کنید &hellip;',
        dropZoneClickTitle: '<br>(or click to select {files})',
        fileActionSettings: {
            removeTitle: 'حذف فایل',
            uploadTitle: 'آپلود فایل',
            zoomTitle: 'دیدن جزئیات',
            dragTitle: 'Move / Rearrange',
            indicatorNewTitle: 'آپلود نشده است',
            indicatorSuccessTitle: 'آپلود شده',
            indicatorErrorTitle: 'بارگذاری خطا',
            indicatorLoadingTitle: 'آپلود ...'
        },
        previewZoomButtonTitles: {
            prev: 'View previous file',
            next: 'View next file',
            toggleheader: 'Toggle header',
            fullscreen: 'Toggle full screen',
            borderless: 'Toggle borderless mode',
            close: 'Close detailed preview'
        },
        //  language: 'fa',
        showUpload: false,
        allowedFileExtensions: ['jpg', 'png'],
        allowedPreviewTypes: ['image'],
        //uploadAsync: true,
        //uploadUrl: '../FileUploader.ashx/ProcessRequest2',
        msgLoading: 'Loading file {index} of {files} …',
        msgProgress: 'Loading file {index} of {files} - {name} - {percent}% completed.',
        previewFileType: 'image'
    });
};


SRL.RenderSelect2 = function (selector) {
    $(selector).select2({
        allowClear: true,
        maximumSelectionSize: 1,
        dir: "rtl"
    });
};


SRL.RequestAds = function (containerId) {
    $('#' + containerId).empty().append('<div class="adSingle"> <button type="button" class="btn btn-block btn-lg" data-toggle="modal" data-target="#newAdModal" data-backdrop="static" onclick="RenderNewAd();">افزودن آگهی جدید</button></div>');
    ServerRequest(true, null, null, true, 'GetUserAds', null, null, function (result) {
        for (var i in result) {
            var ad = result[i];
            RenderSingleAd(containerId, ad);
        }
    });
};


SRL.RequestNewAd = function () {
    if (FormValidation('#newAdForm input,#newAdForm select,#newAdForm button')) {
        var formData = new FormData();
        formData = MakeFormData(formData, '#newAdForm input,#newAdForm select,#newAdForm button');
        formData = MakeImageData(formData, 'newAdPicFiles');
        formData = MakeImageData(formData, 'newAdIconFile');
        FileUploader_ashx(true, null, formData, 'SaveNewAdd', null, null, function (result) {
            alert(result);
        });
        location.reload();
    }
};


SRL.RequestCheckLogin = function (asynchronous) {
    var response = "not set";
    ServerRequest(asynchronous, null, null, true, "CheckLogin", null, null, function (result) {
        response = result;
    });
    return response;
};


SRL.RequestLoginJS = function () {
    if (FormValidation(".loginModal input")) {
        var data = MakeObjectData(".loginModal input");
        alert(data);
        var dataAsJson = JSON.stringify(data);

        alert(dataAsJson);
        ServerRequest(true, null, dataAsJson, true, "TryLogin", null, null, function (result) {
            if (result.loginState == 1) {
                CreateCookie('username', data.usernameLogin);
                CreateCookie('role', result.role);
                location.reload();
            }
            else
                alert("نام کاربری یا رمز عبور اشتباه است یا ثبت نام نکرده اید");
        });
    }
    else alert("نام کاربری یا رمز عبور را وارد نمایید");
};


SRL.RequestLogoutJS = function () {
    ServerRequest(true, null, null, true, 'TryLogout', null, null, function () {
        Redirect('/Default.aspx', null);
        EraseCookie('role');
        EraseCookie('username');
    });
};


SRL.RequestRegisterActivation = function (activationPage, method) {
    var data = {
        username: GetUrlParameter(activationPage.location, "username"),
        activationKey: GetUrlParameter(activationPage.location, "activationKey")
    };
    var dataAsJson = JSON.stringify(data);
    ServerRequest(true, null, dataAsJson, true, method, function () {
        ProgressBar("activationBar", 0, 20, 1);
    },
        function () {
            ProgressBar("activationBar", 20, 50, 1);
        },
        function (result) {
            var targetLabel = $("#activationBoardLabel");
            ProgressBar("activationBar", 50, 100, 1);
            if (result == "activated") {
                targetLabel.text("حساب کاربری شما با موفقیت فعال شد. اکنون وارد حساب کاربری خود می شوید");
                CreateCookie('username', data.username);
                Redirect('/Account/Dashboard.aspx', null);
            }
            else if (result == "activatedBefore") {
                targetLabel.text("حساب کاربری شما قبلا فعال شده و می توانید وارد حساب کاربری خود شوید");
            }
            else if (result == "noUser") {
                targetLabel.text("برای شما حساب کاربری وجود ندارد ابتدا باید ثبت نام کنید");
            }
            else if (result == "wrongActivationKey") {
                targetLabel.text("کد فعالسازی اشتباه است");
            }
            else {
                alert(result);
            }
        }
    );
};


SRL.RequestRegisteration = function () {
    if (FormValidation(".register input")) {// || FormValidation(".register input")) {//:not(#username)
        var data = MakeObjectData(".register input, .register select");
        var dataAsJson = JSON.stringify(data);
        ServerRequest(true, null, dataAsJson, true, 'UserRegisteration', null, null, function (result) {
            if (result.usernameDuplicate) alert(". نام کاربری تکراری است لطفا نام کاربری جدید انتخاب نمایید یا وارد سایت شوید");
            else if (result.emailSent) alert("ثبت نام با موفقیت انجام شد. ایمیل تایید برای شما ارسال گردید لطفا به ایمیل خود مراجعه کرده و با کلیک روی لینک، حساب کاربری خود را فعال کنید");
            else if (result.insertResult && result.emailSent == false) alert("ثبت نام با موفقیت انجام شد. ایمیل تایید برای شما ارسال نشد لفطا وارد حساب کاربری خود شده و اقدام به ارسال مجدد لینک فعالسازی حساب به ایمیل خود کنید");
            else alert("ثبت نام انجام نشد لفطا مجددا سعی کنید");
        });
    }
    else alert("فیلدهای اجباری -ستاره دار- را تکمیل نمایید");
};


SRL.RequestParentChilds = function (parentId) {
    var data = new Object();
    data['parentId'] = parentId;
    var childs = new Array();
    ServerRequest(false, null, JSON.stringify(data), true, 'GetParentChilds', null, null, function (result) {
        if (result.length > 0) {
            for (var i in result) {
                childs.push({
                    text: result[i].childName,
                    tags: [result[i].childId],
                    nodes: RequestParentChilds(result[i].childId)
                });
            }
        }
        else childs = null;
    });
    return childs;
};


SRL.RequestCategoryList = function (async) {
    var parents = new Array();
    var category = new Array();
    var mainNode;
    ServerRequest(async, null, null, true, 'GetCategoryParents', null, null, function (result) {
        for (var i in result) {
            var parentName = result[i].parentName;
            var parentId = result[i].parentId;
            if (parentId > 0)
                parents.push({
                    text: result[i].parentName,
                    tags: [result[i].parentId],
                    nodes: RequestParentChilds(result[i].parentId)
                });
            else mainNode = parentName;
        }
    });
    category.push({ text: mainNode, tags: [0], nodes: parents });
    return category;
};


SRL.RequestAddCategory = function () {
    if (FormValidation('#tbCategoryMgmtNodeName')) {
        var parentNode = $('#categoryMgmtTree').treeview('getSelected');
        var parentTag = parentNode[0].tags;
        var parentId = parseInt(parentTag);
        var childName = $('#tbCategoryMgmtNodeName').val();
        var data = new Object();
        data['childName'] = childName;
        data['parentId'] = parentId;
        ServerRequest(false, null, JSON.stringify(data), true, 'AddCategoryNode', null, null, null);
        RenderCategoryMgmt();
    }
};


SRL.RequestEditCatName = function () {
    if (FormValidation('#tbCategoryMgmtNodeName')) {
        var node = $('#categoryMgmtTree').treeview('getSelected');
        var nodeTag = node[0].tags;
        var nodeId = parseInt(nodeTag);
        var newName = $('#tbCategoryMgmtNodeName').val();
        var data = new Object();
        data['nodeId'] = nodeId;
        data['nodeName'] = newName;
        ServerRequest(true, null, JSON.stringify(data), true, 'EditCatName', null, null, function () {
            RenderCategoryMgmt();
        });
    }
};


SRL.RequestDeleteNode = function () {
    var node = $('#categoryMgmtTree').treeview('getSelected');
    var nodeTag = node[0].tags;
    var nodeId = parseInt(nodeTag);
    if (nodeId > 0) {
        var data = new Object();
        data['nodeId'] = nodeId;
        ServerRequest(true, null, JSON.stringify(data), true, 'DeleteCategory', null, null, function () {
            RenderCategoryMgmt();
        });
    }
};


SRL.Redirect = function (page, data) {//page start with / end with .aspx or else, data is jason
    // var data = {
    //    username: 'one',
    //    userid: '1'
    //};
    var nextUrl = window.location.protocol + "//" + window.location.host + page;
    if (data) nextUrl += "?" + $.param(data);
    window.open(nextUrl, '_self');
};


SRL.RedirectIfLoginServer = function (page, asynchronous, data, notLoginCallBack) {//page start with / end with .aspx or else, data is jason: RedirectIfLoginServer('/Account/Dashboard.aspx', false, null, null)
    var logStatus = RequestCheckLogin(asynchronous);
    if (logStatus.isLogin) {
        Redirect(page, data);
        return true;
    }
    else {
        if (notLoginCallBack) notLoginCallBack()
        return false;
    }
};


SRL.RedirectIfLoginClient = function (page, data, notLoginCallBack) {
    var logStatus = ReadCookie('username');
    if (logStatus) {
        Redirect(page, data);
        return true;
    }
    else {
        if (notLoginCallBack) notLoginCallBack()
        return false;
    }
};


SRL.RedirectIfNotLoginServer = function (page, asynchronous, data, loginCallBack) {
    var logStatus = RequestCheckLogin(asynchronous);
    if (logStatus.isLogin == false) Redirect(page, data);
    else if (loginCallBack) loginCallBack(logStatus.username);
};


SRL.RedirectIfNotLoginClient = function (page, data, loginCallBack) {
    var logStatus = ReadCookie('username');
    if (!logStatus) Redirect(page, data);
    else if (loginCallBack) loginCallBack(logStatus);
};


SRL.ReadCookie = function (name) {
    var nameEQ = encodeURIComponent(name) + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) === ' ') c = c.substring(1, c.length);
        if (c.indexOf(nameEQ) === 0) return decodeURIComponent(c.substring(nameEQ.length, c.length));
    }
    return null;
};


SRL.RemoveAddCssClass = function (selector, removeClass, addClass) {//RemoveAddCssClass('#' + fileContainerId, 'show', 'hide');
    $(selector).each(
        function (i) {
            var target = $(this);
            if (removeClass) {

                if (target.hasClass(removeClass)) target.removeClass(removeClass);
            }
            if (addClass) {

                if (!target.hasClass(addClass)) target.addClass(addClass);
            }
        }
    );
};


SRL.ResetFileInput = function (inputFileId, labelId, labelTxt, btnFileId, btnTxt, fileId, fileContainerId, fileCloseId) {
    if (inputFileId) $('#' + inputFileId).val(null);
    if (fileCloseId) RemoveAddCssClass('#' + fileCloseId, 'show', 'hide');
    if (fileContainerId) RemoveAddCssClass('#' + fileContainerId, 'show', 'hide');
    if (labelTxt) $('#' + labelId).html(labelTxt).css({ 'color': 'black' });
    if (btnTxt) $('#' + btnFileId).val(null).html(btnTxt);
    if (fileId) $('#' + fileId).attr('src', null);
};


SRL.ScrollLinkSmothOnClick = function (linkSelector, speed) {// ('a[href*="#"]:not([href="#"])', 1000)

    $(linkSelector).click(function () {
        if (location.pathname.replace(/^\//, '') == this.pathname.replace(/^\//, '') && location.hostname == this.hostname) {
            var target = $(this.hash);
            target = target.length ? target : $('[name=' + this.hash.slice(1) + ']');
            if (target.length) {
                $('html, body').animate({
                    scrollTop: target.offset().top
                }, speed);
                return false;
            }
        }
    });

};

SRL.ScrollSmothOnClick = function (click_selector, target_selector, speed) {// ('a[href*="#"]:not([href="#"])', 1000)

    $(click_selector).click(function () {
        $('html, body').animate({
            scrollTop: $(target_selector).offset().top
        }, speed);
    });

};
SRL.ScrollSmothToTopOnClick = function (click_selector, speed) {// ('a[href*="#"]:not([href="#"])', 1000)
    //click_selector=".navbar a, footer a[href='#myPage']"

    $(".navbar a, footer a[href='#myPage']").on('click', function (event) {

        // Make sure this.hash has a value before overriding default behavior
        if (this.hash !== "") {

            // Prevent default anchor click behavior
            event.preventDefault();

            // Store hash
            var hash = this.hash;

            // Using jQuery's animate() method to add smooth page scroll
            // The optional number (900) specifies the number of milliseconds it takes to scroll to the specified area
            $('html, body').animate({
                scrollTop: $(hash).offset().top
            }, speed, function () {

                // Add hash (#) to URL when done scrolling (default click behavior)
                window.location.hash = hash;
            });
        } // End if
    });

};


SRL.ServerRequest = function (asynchronous, isValid, dataAsJson, isUrlDefault, methodOrNewUrl, beforeCallBack, completeCallBack, successCallBack) {
    var url = "../PublicForm.aspx/";
    if (isUrlDefault) url += methodOrNewUrl;
    else
        url = methodOrNewUrl;
    $.ajax({
        async: asynchronous,
        type: "post",
        data: dataAsJson,
        url: url,
        contentType: "application/json; charset=utf-8",//ajax input
        dataType: 'json',//ajax output
        beforeSend: function (jqXHR) {
            if (beforeCallBack) beforeCallBack();
        },
        success: function (data, textStatus, jqXHR) {
            if (data.d) {
                if (data.d.redirect) Redirect(data.d.redirect, data.d.redirectData);
                else successCallBack(data.d);
            }
            else successCallBack(data.d);
        },
        error: function (response, x, y) {
            alert(response.responseText);
            if (isValid) isValid.valid *= 0;
        },
        complete: function (jqXHR, textStatus) {
            if (completeCallBack) completeCallBack();
        }
    });
};


SRL.SetTagData = function (tagSelector, dataName, dataObj) {
    $(tagSelector).data(dataName, dataObj);
};


SRL.SetCarouselHeight = function (carousel_selector, height) {
    $(carousel_selector).css('max-height', height);
    $(carousel_selector+ ' img').css('height', height);
};

SRL.SetCarouselImages = function (carousel_selector, pics_url) {
   // var pics = ["image/pmlite2.jpg" , "image/pmlite3.jpg"]
    $(carousel_selector + ' .carousel-inner').empty();
    for (var i = 0 ; i < pics_url.length ; i++) {
        $('<div class="item"><img  width="1200" max-height="700" src="' + pics_url[i] + '">   </div>').appendTo(carousel_selector + ' .carousel-inner');
    }
    $(carousel_selector+' .item').first().addClass('active');
    $(carousel_selector).carousel();
};

SRL.SetClickEvent = function (fromId, toId) {
    $('#' + fromId).click(function (e) {
        $('#' + toId).trigger('click');
    });
  
};



SRL.SelectCategory = function (treeId, targetId) {
    var selectedNode = $('#' + treeId).treeview('getSelected');
    $('#' + targetId).text(selectedNode[0].text);
    //SetTagData('#' + targetId, 'categoryId', selectedNode[0].tags);
    $('#' + targetId).val(selectedNode[0].tags);
    FieldValidation($('#' + targetId), true);
};


SRL.StartLoader = function (toHideSelector, loaderSelector, disableSelector) {//
    if (toHideSelector) $(toHideSelector).hide();
    if (loaderSelector) $(loaderSelector).css('display', 'inline-block');
    if (disableSelector) $(disableSelector).attr('disabled', true);
};



(function ($) {
    "use strict"; // Start of use strict

    $("#addNewItem").click(function () {

        let ColorImageItems = $(".ColorImageItem");
        let firstItem = $(ColorImageItems[0].cloneNode(true));
        let formControls = firstItem.find(".form-control");

        formControls[0].setAttribute("name", `[${ColorImageItems.length}].ColorId`);
        formControls[1].setAttribute("name", `[${ColorImageItems.length}].Image`);
        formControls[1].value = "";

        let itemContainer = $(".itemContainer");
        itemContainer.append(firstItem);
    });

})(jQuery);

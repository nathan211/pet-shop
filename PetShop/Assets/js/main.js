$(document).ready(function () {
    CKEDITOR.replace('Description');
    $('#selectImage').click(function () {
        var finder = new CKFinder();
        finder.selectActionFunction = function (fileUrl) {
            $('#Image').val(fileUrl);
        };
        finder.popup();
    });  
});
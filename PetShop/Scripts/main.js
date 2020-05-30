$(document).ready(function () {
    CKEDITOR.replaceAll('description');
    
    $('#selectImage').click(function () {
        var finder = new CKFinder();
        finder.selectActionFunction = function (fileUrl) {
            $('#Image').val(fileUrl);
        };
        finder.popup();
    });

   
});
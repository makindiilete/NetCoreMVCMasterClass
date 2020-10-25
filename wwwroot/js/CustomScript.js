/*our confirmDelete function which takes 2 args : uniqueId : - ds is the userId "@user.id" that is unique to every user... isDeleteClicked : - this params returns a boolean value which determines if user clicks "Delete" button or "No" on the displayed delete dialog. if user click on the delete, it returns "true", on the display if user clicks on "No", it returns "false"*/
function confirmDelete(uniqueId, isDeleteClicked) {
    //we compute the unique value for our deleteSpan element
    var deleteSpan = 'deleteSpan_' + uniqueId;
    //we compute the unique value for our confirmDeleteSpan
    var confirmDeleteSpan = 'confirmDeleteSpan_' + uniqueId;

    //if the "delete" button is clicked
    if (isDeleteClicked) {
        //we hide the delete button
        $('#' + deleteSpan).hide();
        //and we show "confirmDeleteSpan" that contains the Yes & NO
        $('#' + confirmDeleteSpan).show();
        //else if the user clicks on "No" button on the confirm delete dialog
    } else {
        //we show the delete button
        $('#' + deleteSpan).show();
        //and we hide the confirmDelete dialog
        $('#' + confirmDeleteSpan).hide();
    }
}

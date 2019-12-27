function showDialog(title, body) {
    $('#modal-title').html(title);
    $('#modal-body').html(body);
    $('#modal').modal();
}
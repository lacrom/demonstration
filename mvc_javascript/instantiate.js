$(function () {
    //when page is loaded instantiate model, view and controller
    //fill model with list of tasks
    var model = new CollectionModel(['buy a car', 'go to gym', 'walk 10 min a day']),
        view = new CollectionView(model, {
            'list' : $('#list'), 
            'addButton' : $('#addButton'), 
            'removeButton' : $('#removeButton')
        }),
        controller = new CollectionController(model, view);
    
    //show the list
    view.show();
})
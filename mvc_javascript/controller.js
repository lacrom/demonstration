function CollectionController(model, view) {
    this._model = model;
    this._view = view;

    //store instance for use in functions
    var _this = this;

    this._view.listModified.attachListener(function (source, inputData) {
        _this.updateSelected(inputData.index);
    });

    this._view.addButtonClicked.attachListener(function () {
        _this.addItem();
    });

    this._view.removeButtonClicked.attachListener(function () {
        _this.removeItem();
    });
}

CollectionController.prototype = {
    addItem: function () {
        //get text of new Task
        var item = $('#newItem').val();
        
        //if text is not empty add to list and clear the field
        if (item) {
            this._model.addToList(item);
            $('#newItem').val('');
        }
    },
    
    removeItem: function () {
        //get index of task to remove
        var index = this._model.getSelectedIndex();
        
        //if something selected then remove
        if (index !== -1) {
            this._model.removeItemAt(index);
        }
    },

    updateSelected: function (index) {
        this._model.setSelectedIndex(index);
    }
};
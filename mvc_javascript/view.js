function CollectionView(model, elements) {
    this._model = model;
    this._elements = elements;

    this.listModified = new Event(this);
    this.addButtonClicked = new Event(this);
    this.removeButtonClicked = new Event(this);

    var _this = this;

    //attach listeners to model so view do its work when something changed 
    this._model.itemAdded.attachListener(function () {
        _this.updateList();
    });
    this._model.itemRemoved.attachListener(function () {
        _this.updateList();
    });

    //attach handlers to buttons and list
    this._elements.list.change(function (e) {
        _this.listModified.notify({ index : e.target.selectedIndex });
    });
    this._elements.addButton.click(function () {
        _this.addButtonClicked.notify();
    });
    this._elements.removeButton.click(function () {
        _this.removeButtonClicked.notify();
    });
}

CollectionView.prototype = {
    show : function () {
        //update list when its time to show
        this.updateList();
    },

    updateList : function () {
        //get <select> element and clear its content
        var listElement = this._elements.list;
        listElement.html('');

        var listItems = this._model.getList();
        
        //for each item in list add option to select element
        _(listItems).each(function(elem) {
           listElement.append($('<option>' + elem + '</option>')); 
        });
        
        //clear selection
        this._model.setSelectedIndex(-1);
    }
};
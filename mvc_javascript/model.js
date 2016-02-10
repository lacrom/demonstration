function CollectionModel(listOfItems) {
    this._list = listOfItems;
    this._selectedIndex = -1;

    this.itemAdded = new Event(this);
    this.itemRemoved = new Event(this);
    this.selectedIndexChanged = new Event(this);
}

CollectionModel.prototype = {
    getList: function () {
        //create new array
        return [].concat(this._list);
    },

    addToList: function (item) {
        this._list.push(item);
        //broadcast event
        this.itemAdded.notify({ item: item });
    },

    removeItemAt: function (index) {
        var item = this._list[index];
        this._list.splice(index, 1);
        
        //broadcast event that item was removed
        this.itemRemoved.notify({ item: item });
        
        if (index === this._selectedIndex) {
            this.setSelectedIndex(-1);
        }
    },

    getSelectedIndex: function () {
        return this._selectedIndex;
    },

    setSelectedIndex: function (index) {
        //set new index and notify
        var previousIndex = this._selectedIndex;
        this._selectedIndex = index;
        this.selectedIndexChanged.notify({ previous: previousIndex });
    }
};

function Event(source) {
    this._source = source;
    this._listeners = [];
}

Event.prototype = {
    attachListener: function (listener) {
        //add new listener to array of listeners
        this._listeners.push(listener);
    },
    notify: function (inputData) {
        //let each listener work with input data
        _(this._listeners).each(function(listener) {
             listener(this._source, inputData);
        });
    }
};
/* **************************** EditIndex.js ********************************************* */
// Makes an HTML table editable, and saves the changes to the server
// Requires: JQuery

// Table should be in the following form, adding model-id, field-name, and field-type properties:
/*
    <table>
        <tr>
            <th></th>
            <th></th>
        </tr>
        <tr model-id="@item.Id">
            <td field-name="Name" field-type="string"></td>
            <td></td>
        </tr>
    </table>
*/
// Valid field-type values are "string", "int", "decimal", and "boolean"


// HTML should also have the following button to toggle edit mode
/*
<button onclick="editIndex.toggleEditable(this, '/Products/Update');">Edit Values As Table</button>
*/

// Controller should look something like this:
/*
        /// <summary>
        /// Updates the model from the Index table using EditIndex.js
        /// </summary>
        /// <param name="id">The Id of the model to update</param>
        /// <returns>A Json object indicating success status.  In case of error, returns object with data member containing the old product model,
        /// and the field causing the error if possible</returns>
        [HttpPost]
        public async Task<JsonResult> Update(int id)
        {
            Product existingProduct = await db.Products.FindAsync(id);
            if (existingProduct == null)
            {
                return this.JError(404, "Can't find this product to update!");
            }

            try
            {
                Product newProduct = JsonConvert.DeserializeObject<Product>(Request.Params.Get("data"));
                newProduct.Id = id;

                db.Entry(existingProduct).State = EntityState.Detached;
                db.Entry(newProduct).State = EntityState.Modified;
                await db.SaveChangesAsync();
            }
            catch (JsonReaderException e)
            {
                var returnData = JsonConvert.SerializeObject(new { product = existingProduct, error = e.Message, field = e.Path});
                return this.JError(400, "Invalid data!", returnData);
            }
            catch (Exception e)
            {
                var returnData = JsonConvert.SerializeObject(new { product = existingProduct, error = e.Message });
                return this.JError(400, "Unable to save!", returnData);
            }
            return this.JOk();
        }
*/


var editIndex = {
    // Initialize model
    init: function () {
        this.$editableRows = $('tr[model-id]');
        this.$editableElements = this.$editableRows.find('td[field-name]');
        this.isEditable = false;
        this.buttonLabel = '';
        this.postUrl = '';

        // Prevent focusout event double fire on switch tabs
        this.prevActiveElement;
        $(window).on('blur', function () {
            prevActiveElement = document.activeElement;
        });
    },

    // Called by toggle button onclick to toggle the editable state
    // button: the toggle button (this)
    // postUrl: the server url to which to post the update data
    toggleEditable: function (button, postUrl) {
        this.postUrl = postUrl;
        this.isEditable = !this.isEditable;

        // jquery containers for editable fields
        var $editableStrings = this.$editableRows.find('td[field-type="string"]');
        var $editableInts = this.$editableRows.find('td[field-type="int"]');
        var $editableDecimals = this.$editableRows.find('td[field-type="decimal"]');
        var $editableBooleans = this.$editableRows.find('td[field-type="boolean"]');

        if (this.isEditable) {
            // change button label, saving original label
            this.buttonLabel = button.textContent;
            button.textContent = 'Done';

            // make fields editable
            $.each($editableStrings, this.turnOnEditText);
            $.each($editableInts, this.turnOnEditText);
            $.each($editableDecimals, this.turnOnEditText);
            $.each($editableBooleans, this.turnOnEditCheckbox);

            // save data when leaving row
            this.$editableRows.on('focusout', this.saveData);

            // prevent double focusout from firing when switching tabs
            this.$editableRows.on('focusin', function () { editIndex.prevActiveElement = document.activeElement; });
        }
        else {
            // restore original button label
            button.textContent = this.buttonLabel;

            // make fields uneditable
            $.each($editableStrings, this.turnOffEditText);
            $.each($editableInts, this.turnOffEditText);
            $.each($editableDecimals, this.turnOffEditText);
            $.each($editableBooleans, this.turnOffEditCheckbox);
        }
    },

    // Called by $.each in toggleEditable to make an individual field editable
    turnOnEditText: function (index, element) {
        var self = editIndex;
        var $element = $(element);
        // insert a div inside the td containing the original contents of the td
        var originalHtml = $element.html();
        $element.html('');
        $element.append($('<div class="input-text"></div>'));
        var $elementInnerDiv = $element.find('.input-text');
        $elementInnerDiv.html(originalHtml);

        // make the inner div editable
        $elementInnerDiv.attr('contenteditable', 'true');

        $elementInnerDiv.on('keydown', self.processKeys);
    },

    // Called by $.each in toggleEditable to make an individual field uneditable
    turnOffEditText: function (index, element) {
        var $element = $(element);

        // remove the inner, editable div, transfer contents back to the simple td
        var $elementInnerDiv = $element.find('.input-text');
        var originalHtml = $elementInnerDiv.html();
        $element.html(originalHtml);
    },

    // Called by $.each in toggleEditable to make a checkbox editable
    turnOnEditCheckbox: function (index, element) {
        var self = editIndex;
        var $element = $(element);
        var $elementInnerDiv = $element.find('input[type="checkbox"]');
        $elementInnerDiv.addClass('input-text');
        $elementInnerDiv.attr('disabled', false);
        $elementInnerDiv.on('keydown', self.processKeys);
    },

    // Called by $.each in toggleEditable to make a checkbox uneditable
    turnOffEditCheckbox: function (index, element) {
        var $element = $(element);
        var $elementInnerDiv = $element.find('input[type="checkbox"]');
        $elementInnerDiv.removeClass('input-text');
        $elementInnerDiv.attr('disabled', true);
    }, 

    // Allow traversing up or down through the rows
    processKeys: function (e) {
        if (e.key === 'ArrowDown' || e.key === 'ArrowUp') {
            var $td = $(e.currentTarget).closest('td');
            var cellIndex = $td[0].cellIndex;
            var $newRow
            if (e.key === 'ArrowDown') {
                $newRow = $($td.closest('tr')[0].nextElementSibling);
            }
            else if (e.key === 'ArrowUp') {
                $newRow = $($td.closest('tr')[0].previousElementSibling);
            }

            var $newTd = $($newRow.find('td')[cellIndex]);
            var $newCell = $newTd.find('.input-text');
            $newCell.focus();
            e.preventDefault();
        }
    },

    // Save the data in a row to the server
    saveData: function (e) {
        // only save when leave row
        var $target = $(e.currentTarget);
        var $active = $(e.relatedTarget);
        if ($active.closest($target).length !== 0) {
            return;
        }

        // reference to main object to pass to enclosures
        var self = editIndex;

        // prevent double fire on switch tabs
        if (document.activeElement === self.prevActiveElement) {
            return;
        }
        self.prevActiveElement = document.activeElement;

        // clear error borders that may have been set previously
        self.clearColorBorderClasses($target);

        // create data object to post to server
        var data = {};
        self.getTextToSave($target, data);
        self.getCheckboxesToSave($target, data);
        var id = $target.attr('model-id');
        var postData = {
            data: JSON.stringify(data)
        };

        // post the data to server
        $.post(self.postUrl + '/' + id, postData, function (response) {
            // add green border on row to indicate success
            $target.addClass('green-border');
            // setTimeout to give green border a chance to register before starting the fade
            setTimeout(function () {
                $target.addClass('border-fade');
                setTimeout(self.clearColorBorderClasses, 5000, $target);
            }, 10);
        }).fail(function (xhr, status, error) {
            // get error data from response
            var returnData = JSON.parse(xhr.responseJSON.data);
            var errorField = returnData.field;
            var errorMessage = xhr.responseJSON.message;
            var product = returnData.product;
            var target = $('tr[model-id="' + product.ProductId + '"]');

            // add red border on row or field to indicate failure
            var $errorField;
            if (errorField) {
                $errorField = $(target).find('td[field-name="' + errorField + '"]').find('.input-text');
            }
            else {
                $errorField = $(target);
            }
            $errorField.addClass('red-border');
            alert('Error: ' + errorMessage);

            // set focus back to field with error
            setTimeout(function () {
                $errorField.focus();
            }, 10);
        });
    },

    // Called by saveData to add text data fields to object to post to server
    getTextToSave: function ($target, data) {
        var $rowData = $target.find('.input-text');
        $.each($rowData, function (value, element) {
            var $element = $(element);
            var $td = $element.closest('td');
            var fieldName = $td.attr('field-name');
            data[fieldName] = $element.text().trim();
        });
    },

    // Called by saveData to add boolean data fields to object to post to server
    getCheckboxesToSave: function ($target, data) {
        var $rowData = $target.find('input[type="checkbox"]');
        $.each($rowData, function (value, element) {
            var $element = $(element);
            var $td = $element.closest('td');
            var fieldName = $td.attr('field-name');
            data[fieldName] = $element.prop('checked');
        });
    },

    // Removes success or error color borders from field or row
    clearColorBorderClasses: function ($target) {
        $target.find('.input-text').removeClass('red-border');
        $target.removeClass('red-border');
        $target.removeClass('green-border');
        $target.removeClass('border-fade');
    }
};

editIndex.init();
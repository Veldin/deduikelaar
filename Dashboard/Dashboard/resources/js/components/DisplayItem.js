import React, {Component} from 'react';

class CreateItem extends Component {
    render() {
      return (
        <div>
        <h1>Maak een item aan</h1>
        <form>
          <div className="row">
            <div className="col-md-6">
              <div className="form-group">
                <label>Titel:</label>
                <input type="text" className="form-control" />
              </div>
            </div>
            </div>
        </form>
  </div>
      )
    }
}
export default CreateItem;
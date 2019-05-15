import React, {Component} from 'react';
import { Router, Route, Link } from 'react-router';

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
            <div className="row">
              <div className="col-md-6">
                <div className="form-group">
                  <label>Beschrijving:</label>
                  <input type="text" className="form-control col-md-6" />
                </div>
              </div>
            </div><br />
            <div className="form-group">
              <button className="btn btn-primary">Opslaan</button>
            </div>
        </form>
  </div>
      )
    }
}
export default CreateItem;
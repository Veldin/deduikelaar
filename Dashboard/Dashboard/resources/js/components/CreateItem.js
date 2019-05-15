import React, {Component} from 'react';
import { Router, Route, Link } from 'react-router';

class CreateItem extends Component {
    render() {
      return (
      <div className="container-fluid">
        <div className="row">
          <div className="col s6 inputFields" >
        <form>
        <div className="row">
          <div class="input-field col s6">
            <input id="last_name" type="text"></input>
            <label for="last_name">Titel</label>
          </div>
          </div>
        </form>

          </div>



          <div className="col s6 inputFields">
          </div>
        </div>
      </div>
      )
    }
}
export default CreateItem;
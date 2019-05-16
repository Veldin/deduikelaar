import React, {Component} from 'react';
import { Router, Route, Link } from 'react-router';

class Overview extends Component {
  render(){
    return (
      <div className="row overviewFilter">
        <div className="col s2 overviewLabel">Alle Items</div>
        <div className="col s10 overviewSwitch">
          <div className="switch">
            <label>
              Alleen Actief
              <input type="checkbox" checked></input> //TODO: checkbox cant uncheck
              <span className="lever"></span>
              Alle
            </label>
          </div>
        </div>
      </div>
    )
  }
}
export default Overview;
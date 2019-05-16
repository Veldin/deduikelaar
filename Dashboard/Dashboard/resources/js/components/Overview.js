import React, {Component} from 'react';
import { Router, Route, Link } from 'react-router';

class Overview extends Component {
  render(){
    return (
      <div class="container">
        <div class="row">
          <div class="col s2 navigationContainer">
            <Link class="col s12 navigationItem" activeClassName="col s12 navigationItem active" to="/overview">
              <span class="navigationItemText">Overzicht</span>
            </Link>
            <Link class="col s12 navigationItem" activeClassName="col s12 navigationItem active" to="/add-item">
              <span class="navigationItemText">Item Toevoegen</span>
            </Link>
            <Link class="col s12 navigationItem" activeClassName="col s12 navigationItem active" to="/see-example">
              <span class="navigationItemText">Voorbeeld</span>
            </Link>
          </div>
          <div class="col s1"></div>
          <div class="col s9 contentContainer">2</div>
        </div>
      </div>
    )
  }
}
export default Overview;
import React, {Component} from 'react';
import { Router, Route, Link } from 'react-router';

class Overview extends Component {
  render(){
    return (
      <div class="container">
        <div class="row">
          <div class="col s3 navigationContainer">
            <Link class="col s12 navigationItem" to="/overview">Overzicht</Link>
            <Link class="col s12 navigationItem" to="/add-item">Item Toevoegen</Link>
            <Link class="col s12 navigationItem" to="/see-example">Voorbeeld</Link>
          </div>
          <div class="col s9 overviewContainer">2</div>
        </div>
      </div>
    )
  }
}
export default Overview;
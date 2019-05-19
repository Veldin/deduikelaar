import React, {Component} from 'react';
import { Router, Route, Link } from 'react-router';

class Master extends Component {
  render(){
    return (
      <div className="container">
        <div className="row content">
          <div className="col s2 navigationContainer">
            <Link className="col s12 navigationItem" activeClassName="col s12 navigationItem active" to="/overview">
              <span className="navigationItemText">Overzicht</span>
            </Link>
            <Link className="col s12 navigationItem" activeClassName="col s12 navigationItem active" to="/add-item">
              <span className="navigationItemText">Item Toevoegen</span>
            </Link>
            <Link className="col s12 navigationItem" activeClassName="col s12 navigationItem active" to="/see-example">
              <span className="navigationItemText">Voorbeeld</span>
            </Link>
          </div>
          <div className="col s1"></div>
          <div className="col s9 contentContainer">
            {this.props.children}
          </div>
        </div>
      </div>
    )
  }
}
export default Master;
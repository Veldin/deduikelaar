import React, {Component} from 'react';
import { Router, Route, Link } from 'react-router';

class Master extends Component {

  componentDidMount() {
    if(window.location.pathname == '/'){
      document.getElementById("overview").click();
    }
  }

  render(){
    return (
      <div>
        <div className="header">
          <div className="row">
            <div className="col s8">
              <div className="logoHeader"></div>
            </div>
            <div className="input-field col s4">
              <Link className="buttonHeader" to="/add-item">
                <button className="btn waves-effect waves-light createItemButton" type="submit" name="action">Item toevoegen</button>
              </Link>
            </div>
          </div>
        </div>
        <div className="container">
          <div className="row content">
            <div className="col s12 m2 l2 navigationContainer">
              <Link className="col s6 m12 l12 navigationItem" activeClassName="col s12 navigationItem active" to="/overview">
                <span className="navigationItemText" id="overview">Overzicht</span>
              </Link>
              <Link className="col s6 m12 l12 navigationItem" activeClassName="col s12 navigationItem active" to="/statistics">
                <span className="navigationItemText">Statestieken</span>
              </Link>
            </div>
            <div className="col s1"></div>
            <div className="col s12 m9 l9 contentContainer">
              {this.props.children}
            </div>
          </div>
        </div>
      </div>
    )
  }
}
export default Master;
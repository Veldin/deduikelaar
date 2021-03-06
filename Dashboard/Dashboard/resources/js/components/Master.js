import React, {Component} from 'react';
import { Router, Route, Link } from 'react-router';

// This class dictates the default structure of every page, containing navigation and header
class Master extends Component {

  // This function gets executed when the page loads
  componentDidMount() {
    // Forwards the page to overview else an empty page would show
    if(window.location.pathname == '/'){
      document.getElementById("overview").click();
    }
  }

  // Render the navigation and header
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
              {/* Here all the page content will be loaded */}
              {this.props.children}
            </div>
          </div>
        </div>
      </div>
    )
  }
}
export default Master;
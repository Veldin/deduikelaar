import React, {Component} from 'react';
import { Router, Route, Link } from 'react-router';

class Master extends Component {
  render(){
    return (
      <div className="container">
        <nav className="navbar navbar-default">
          <div className="container-fluid">
            <div className="navbar-header">
              <a className="navbar-brand" href="#">Dashboard</a>
            </div>
            <ul className="nav navbar-nav">
              <Link to="/add-item">Item toevoegen</Link>
              <Link to="/see-item">Overview</Link>
              <Link to="/see-example">Example</Link>
            </ul>
          </div>
      </nav>
          <div>
              {this.props.children}
          </div>
      </div>
    )
  }
}
export default Master;
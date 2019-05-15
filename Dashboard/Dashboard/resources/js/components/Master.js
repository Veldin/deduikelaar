import React, {Component} from 'react';
import { Router, Route, Link } from 'react-router';

class Master extends Component {
  render(){
    return (
      <div>
        <Link to="/overview">1</Link>
        <Link to="/add-item">2</Link>
        <Link to="/see-example">3</Link>
        {this.props.children}
      </div>
    )
  }
}
export default Master;
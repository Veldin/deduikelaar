import React, {Component} from 'react';
import { Router, Route, Link } from 'react-router';

class Overview extends Component {
  render(){
    return (
      <div class="container">
        <div class="row">
          <div class="col s3 navigationContainer">1</div>
          <div class="col s9 overviewContainer">2</div>
        </div>
      </div>
    )
  }
}
export default Overview;
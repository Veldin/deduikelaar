import React, {Component} from 'react';
import { Router, Route, Link } from 'react-router';
import Card from './Card/Card';

class Overview extends Component {
  constructor() {
    super();

    this.state = {
      card: []
    }
  }

  componentDidMount() {
    fetch('/api/v1/stories')
      .then((response) => response.json())
      .then((responseJson) => {
        this.setState({ 
          card: responseJson 
        })
        console.log(this.state.card);
      })
  }

  render(){
    return (
      <div className="overviewContentContainer">
        <div className="row overviewFilter">
          <div className="col s2 overviewLabel">Alle Items</div>
          <div className="col s10 overviewSwitch">
            <div className="switch">
              <label>
                Alleen Actief
                <input type="checkbox" defaultChecked></input>
                <span className="lever"></span>
                Alle
              </label>
            </div>
          </div>
        </div>
        <div className="row overviewCards">
          <div className="cards-container">
            {this.state.card.map(item => (
              <Card storyID={item.storyId} title="De brief van Karel" active="1" emoteOne="4" emoteTwo="2" emoteThree="10" />
            ))}
          </div>
        </div>
      </div>
    )
  }
}
export default Overview;
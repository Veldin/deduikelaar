import React, {Component} from 'react';
import Card from './Card/Card';

class Overview extends Component {
  constructor() {
    super();

    this.state = {
      card: []
    }
  }

  componentDidMount() {
    fetch('/api/v1/overview')
      .then((response) => response.json())
      .then((responseJson) => {
        this.setState({ 
          card: responseJson['response']
        })
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
            {this.state.card.map((item, key) =>
              <Card 
                key={key} 
                storyID={item.storyId} 
                title={item.title} 
                active={item.active}
                cardInfo={[this.state.card]}
              />
            )}
          </div>
        </div>
      </div>
    )
  }
}
export default Overview;
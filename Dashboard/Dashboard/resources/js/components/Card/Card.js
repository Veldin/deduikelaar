import React from 'react';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import { faTimes } from '@fortawesome/free-solid-svg-icons'

const card = (props) => {
  return (        
    <div className="col s12 m4">
      <div className="card">
         <div className="card-content white-text">
          <div className="row">
            <div className="col s10">
              <div className="switch">
                <label>
                  Actief
                  <input type="checkbox" checked></input> 
                  <span className="lever"></span>
                  Inactief
                </label>
              </div>
            </div>
            <div className="col s2">
              <div className="deleteItem">
                <FontAwesomeIcon icon={faTimes} />
              </div>
            </div>
          </div>
          <div className="row">
            <div className="col s12">
               <span className="card-title">{props.title}</span>
            </div>
          </div>

           <p>I am a very simple card. I am good at containing small bits of information.
             I am convenient because I require little markup to use effectively.</p>
         </div>
      </div>
    </div>
  )
};

export default card;
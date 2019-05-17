import React from 'react';

const card = (props) => {
  return (        
    <div className="col s12 m4">
      <div className="card blue-grey darken-1">
         <div className="card-content white-text">
           <span className="card-title">{props.title}</span>
           <p>I am a very simple card. I am good at containing small bits of information.
             I am convenient because I require little markup to use effectively.</p>
         </div>
      </div>
    </div>
  )
};

export default card;
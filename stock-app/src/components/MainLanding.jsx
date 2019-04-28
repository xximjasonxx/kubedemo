
import React from 'react';
import { connect } from 'react-redux';

const MainLandingComponent = () => {
    return (
        <div>Hello World</div>
    );
};

const mapStateToProps = (state) => {
    return {};
};

export default connect(mapStateToProps)(MainLandingComponent);

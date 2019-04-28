
import React from 'react';
import { connect } from 'react-redux';
import { HubConnectionBuilder, LogLevel } from '@aspnet/signalr';

class MainLandingComponent extends React.Component {
    constructor(props) {
        super(props);

        this.state = {
            hubConnection: null
        };
    }

    componentDidMount = () => {
        const hubConnection = new HubConnectionBuilder()
            .withUrl("https://localhost:5001/prices")
            .configureLogging(LogLevel.Information)
            .build();

        this.setState({ hubConnection }, () => {
            this.state.hubConnection
                .start()
                .then(() => console.log('Connection established'))
                .catch(err => {
                        console.log('Connection error: ' + err);
                    });
        });
    }

    render() {
        return (
            <div>Hello World</div>
        );
    }
};

const mapStateToProps = (state) => {
    return {};
};

export default connect(mapStateToProps)(MainLandingComponent);

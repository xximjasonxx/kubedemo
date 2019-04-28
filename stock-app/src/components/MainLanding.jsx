
import React from 'react';
import { HubConnectionBuilder, LogLevel } from '@aspnet/signalr';
import {
    LineChart,
    Line,
    CartesianGrid,
    XAxis,
    YAxis
} from 'recharts';
import randomColor from 'randomcolor';

class MainLanding extends React.Component {
    constructor(props) {
        super(props);

        this.state = {
            hubConnection: null,
            stockPrices: [],
            stockSymbols: [],
            lineColors: randomColor({ count: 100, hue: 'green' })
        };

        this.handleConnectionStart = this.handleConnectionStart.bind(this);
    }

    componentDidMount = () => {
        const hubConnection = new HubConnectionBuilder()
            .withUrl("http://192.168.99.100:31000/prices")
            .configureLogging(LogLevel.Information)
            .build();

        this.setState({ hubConnection }, () => {
            this.state.hubConnection
                .start()
                .then(this.handleConnectionStart)
                .catch(err => {
                    console.log('Connection error: ' + err);
                });
        });
    }

    handleConnectionStart() {
        console.log("Connection established");
        this.state.hubConnection.on('ReceiveStockPrice', (stockPrice) => {            
            var dataObject = {
                symbol: stockPrice.symbol,
                price: stockPrice.newPrice,
                publishTime: stockPrice.publishTime };

            var symbols = this.state.stockSymbols;
            if (symbols.filter(symbol => symbol === stockPrice.symbol).length === 0) {
                symbols = [ ...symbols, { symbol: stockPrice.symbol, color: this.state.lineColors[symbols.length] }];
            }

            this.setState({
                stockPrices: [ ...this.state.stockPrices, dataObject ],
                stockSymbols: symbols
            });
        });
    }

    render() {
        const { stockPrices, stockSymbols } = this.state;

        return (
            <div>
                <h2>Stock Prices</h2>
                <LineChart width={400} height={400}>
                    {stockSymbols.map(symObj => {
                        return <Line type="monotone" dataKey="price" data={stockPrices.filter(x => x.symbol === symObj.symbol)} stroke={symObj.color} />
                    })}
                    <CartesianGrid stroke="#ccc" />
                    <XAxis dataKey="publishTime" />
                    <YAxis dataKey="price" />
                </LineChart>
            </div>
        );
    }
};

export default MainLanding;

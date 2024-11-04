import { Outlet, useLocation } from 'react-router-dom';
import Header from '../components/Header/Header';
import Container from '../components/Container/Container';
import Footer from '../components/Footer/Footer';
import { useEffect } from 'react';
import { useDispatch } from 'react-redux';
import { GATEWAY_SEARCH_ADDRESS_ACTIONS } from '../store/gatewaySearchAddressReducer';

export default function MainLayout() {
    const location = useLocation();
    const dispatch = useDispatch();
    useEffect(() => {
        dispatch({ type: GATEWAY_SEARCH_ADDRESS_ACTIONS.RESET_VALUE });
    }, [location.pathname]);
    return (
        <Container>
            <Header />
            <Outlet />
            <Footer />
        </Container>
    );
}

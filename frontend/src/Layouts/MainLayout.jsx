import { Outlet } from 'react-router-dom';
import Header from '../components/Header/Header';
import Container from '../components/Container/Container';
import Footer from '../components/Footer/Footer';

export default function MainLayout() {
    return (
        <Container>
            <Header />
            <Outlet />
            <Footer />
        </Container>
    );
}

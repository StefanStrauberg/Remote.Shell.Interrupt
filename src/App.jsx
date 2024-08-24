import { Route, Routes } from 'react-router-dom';
import Gatewayspage from './pages/Gatewayspage/Gatewaypage';
import MainLayout from './Layouts/MainLayout';
import Gatewaypage from './pages/Gatewaypage/Gatewaypage';
import Assigmentspage from './pages/Assigmentspage/Assigmentspage';
import Form from './components/UI/Form/Form';
import Rulespage from './pages/Rulespage/Rulespage';

function App() {
    return (
        <>
            <Routes>
                <Route path="/" element={<MainLayout />}>
                    <Route index element={<Gatewayspage />} />
                    <Route
                        path="/gateways/:id"
                        element={<Gatewaypage />}
                    ></Route>
                    <Route path="/assigments" element={<Assigmentspage />} />
                    <Route path="/assigments/create" element={<Form />}></Route>
                    <Route path="/assigments/update" element={<Form />}></Route>
                    <Route path="/rules" element={<Rulespage />} />
                    <Route path="*" />
                </Route>
            </Routes>
        </>
    );
}

export default App;

import { Box, Paper, Typography, Chip } from '@mui/material'
import { Port } from '../../../lib/types/NetworkDevices/Port'
import { CheckBox, Fingerprint, Home, Speed } from '@mui/icons-material'

type Props = {
	port: Port
}

export default function NetworkDeviceSubPort({ port }: Props) {
	// const getStatusColor = (status: string) =>
	//   status === "up" ? "success.main" : "error.main";

	const getStatusIconColor = (status: string) =>
		status === 'up' ? 'success.main' : 'disabled'

	const getSpeedText = (speed: number) => {
		if (speed >= 1_000_000_000) return `${speed / 1_000_000_000} Gbps`
		if (speed >= 1_000_000) return `${speed / 1_000_000} Mbps`
		return `${speed / 1_000} Kbps`
	}

	return (
		<Paper elevation={1} sx={{ p: 2, mb: 1, backgroundColor: 'grey.50' }}>
			<Box display='flex' alignItems='center' flexWrap='wrap' gap={2}>
				{/* Interface Name */}
				<Box display='flex' alignItems='center'>
					<Fingerprint
						sx={{ mr: 1, color: getStatusIconColor(port.interfaceStatus) }}
					/>
					<Typography variant='body2' fontWeight='medium'>
						{port.interfaceName}
					</Typography>
				</Box>

				{/* Status */}
				<Box display='flex' alignItems='center'>
					<CheckBox
						sx={{ color: getStatusIconColor(port.interfaceStatus), mr: 0.5 }}
					/>
					<Chip
						label={port.interfaceStatus.toUpperCase()}
						size='small'
						color={port.interfaceStatus === 'up' ? 'success' : 'error'}
						variant='outlined'
					/>
				</Box>

				{/* Speed */}
				<Box display='flex' alignItems='center'>
					<Speed
						sx={{ color: getStatusIconColor(port.interfaceStatus), mr: 0.5 }}
					/>
					<Typography variant='body2'>
						{getSpeedText(port.interfaceSpeed)}
					</Typography>
				</Box>

				{/* MAC Address */}
				<Box display='flex' alignItems='center'>
					<Home
						sx={{ color: getStatusIconColor(port.interfaceStatus), mr: 0.5 }}
					/>
					<Typography variant='body2' fontFamily='monospace'>
						{port.macAddress}
					</Typography>
				</Box>
			</Box>
		</Paper>
	)
}
